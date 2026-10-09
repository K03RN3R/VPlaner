using System;
using System.Collections.Generic;
using System.Text;
using VPlaner.Core.Models;
using VPlaner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;


namespace VPlaner.Infrastructure.Services
{

    /// <summary>
    /// Verwaltet Anwesenheits-Status von Personen zu Terminen.
    /// Lazy-Create: ein Attendance-Eintrag entsteht erst, wenn jemand
    /// seinen Status explizit setzt. Vorher wird "Ausstehend" angenommen.
    /// </summary>
    public class AttendanceService
    {
        private readonly AppDbContext _db;

        public AttendanceService(AppDbContext db) => _db = db;

        ///<summary>
        ///Liefert alle aktiven Personen aus den Abteilunge eines Termins,
        ///zusammen mit ihrem aktuellen Status.
        ///Personen ohne Attendance-Eintrag erscheinen als "Ausstehend".
        /// </summary>
        public async Task<List<AttendanceRow>> GetForEventAsync(int eventId)
        {
            // 1. Die Abteilungs-Ids des Events holen
            var deptIds = await _db.EventDepartments
                .Where(ed => ed.EventId == eventId)
                .Select(ed => ed.DepartmentId)
                .ToListAsync();

            if (deptIds.Count == 0)
                return new List<AttendanceRow>();

            // 2. Alle aktiven Personen holen, die in mindestens einer dieser Abteilungen sind
            var persons = await _db.Persons
                .Where(p => p.IsActive)
                .Where(p => p.PersonDepartments.Any(pd => deptIds.Contains(pd.DepartmentId)))
                .Include(p => p.PersonDepartments)
                    .ThenInclude(pd => pd.Department)
                .OrderBy(p => p.Id)
                .ToListAsync();

            // 3. Bestehende Attendance-Einträge zu diesem Event holen
            var attendances = await _db.Attendances
                .Where(a => a.EventId == eventId)
                .ToDictionaryAsync(a => a.PersonId);

            // 4. Zu Zeilen zusammenbauen - eine pP.
            return persons.Select(p => new AttendanceRow
            {
                PersonId = p.Id,
                PersonName = p.Name,
                Departments = p.PersonDepartments
                        .Where(pd => deptIds.Contains(pd.DepartmentId))
                        .Select(pd => pd.Department.Name)
                        .ToList(),
                Status = attendances.TryGetValue(p.Id, out var a)
                        ? a.Status
                        : AttendanceStatus.Ausstehend,
                UpdatedAt = attendances.TryGetValue(p.Id, out var a2)
                        ? a2.UpdatedAt
                        : null,
                UpdatedBy = attendances.TryGetValue(p.Id, out var a3)
                    ? a3.UpdatedBy
                    : null
            }).ToList();
        }

            ///<summary>
            ///Setzt den Status einer Person zu einem Termin
            ///Legt bei Bedarf einen neuen Attendance-Status an
            /// </summary>
            public async Task SetStatusAsync(int eventId, int personId, AttendanceStatus status, string updatedBy)
            {
                var existing = await _db.Attendances
                    .FirstOrDefaultAsync(a => a.EventId == eventId && a.PersonId == personId);

                if (existing == null)
                {
                    _db.Attendances.Add(new EventAttendance
                    {
                        EventId = eventId,
                        PersonId = personId,
                        Status = status,
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = updatedBy
                    });
                }
                else
                {
                    existing.Status = status;
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.UpdatedBy = updatedBy;
                }

            await _db.SaveChangesAsync();
            }
    }

    ///<summary>
    ///DTO für die Anzeige. 
    ///Hält Person + Status + Metadaten.
    ///Kein EF-Entity; nur Projektion
    /// </summary>
    public class AttendanceRow
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public List<string> Departments { get; set; } = new();
        public AttendanceStatus Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
