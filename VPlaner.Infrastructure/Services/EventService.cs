using System;
using System.Collections.Generic;
using System.Text;
using VPlaner.Core.Models;
using VPlaner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VPlaner.Infrastructure.Services
{
    /// <summary>
    /// Liest Tewrmine aus der DB, optional gefiltert nach Abteilungen
    /// und Zeitfenster (nur anstehend vs. alle)
    /// </summary>
    public class EventService
    {
        private readonly AppDbContext _db;

        public EventService(AppDbContext db) => _db = db;

        ///<summary>
        ///Alle Termine, optional gefiltert
        /// </summary>
        /// <param name="=departmentId">Wenn gesetzt: nur Termine, die diese Abteilung betreffen</param>
        /// <param name="=includeCompleted">Wenn false: nur Termine, die noch nicht vorbei sind</param>
        public Task<List<EventItem>> GetAllAsync(int? departmentId = null, bool includeCompleted = false)
        {
            var q = _db.Events
                       .Include(e => e.EventDepartments)
                            .ThenInclude(ed => ed.Department)
                       .AsQueryable();

            if (departmentId.HasValue)
            {
                var deptId = departmentId.Value;
                q = q.Where(e => e.EventDepartments.Any(ed => ed.DepartmentId == deptId));
            }

            if (!includeCompleted)
            {
                var today = DateTime.Today;
                //Termine mit unbekannten Datum zählen immer als "noch anstehend"
                q = q.Where(e => e.Date == null || e.Date >= today);
            }

            //Sortierung: Termine mit Datum zuerst nach Datum, dann die ohne Datum
            return q.OrderBy(e => e.Date == null)
                    .ThenBy(e => e.Date)
                    .ThenBy(e => e.Time)
                    .ToListAsync();
        }
        public Task<EventItem?> GetAsync(int id) => _db.Events
                                                       .Include(e => e.EventDepartments)
                                                            .ThenInclude(ed => ed.Department)
                                                       .FirstOrDefaultAsync(e => e.Id == id);
    }
}
