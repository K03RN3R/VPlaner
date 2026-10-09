using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VPlaner.Core.Models;
using VPlaner.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using VPlaner.Infrastructure.Data;
using System.Security.Cryptography.X509Certificates;


namespace VPlaner.Infrastructure.Services
{

    /// <summary>
    /// Verwaltet Personen und deren Abteilungen. (M:N)
    /// Löschen erfolgt über Soft-Delete mit IsActive
    /// </summary>
    public class PersonService
    {
        private readonly AppDbContext _db;

        public PersonService(AppDbContext db) => _db = db;

        ///<summary>
        ///Alle Personen (optional nur aktive), inkl. ihrer Abteilungen
        /// Sortiert nach ID ascending
        /// </summary>
        public Task<List<Person>> GetAllAsync(bool includeInactive = false) 
            {
                var q = _db.Persons
                        .Include(p => p.PersonDepartments)
                                .ThenInclude(pd => pd.Department)
                                .AsQueryable();
            if (!includeInactive)
                q = q.Where(p => p.IsActive);
                return q.OrderBy(p => p.Id).ToListAsync();
            }

        ///<summary>
        ///Alle Personen einer Abteilung (optional nur aktive), inkl. ihrer Abteilungen
        ///</summary>
        public Task<List<Person>> GetByDepartmentAsync(int departmentId, bool includeInactive = false)
        {
            var q = _db.PersonDepartments
                        .Where(pd => pd.DepartmentId == departmentId)
                        .Select(pd => pd.Person);
            if (!includeInactive)
                q = q.Where(p => p.IsActive);
            return q.OrderBy(p => p.Id).ToListAsync();
        }

        ///<summary>
        ///Legt eine neue Person an und ordnet sie in die angegebenen Abteilungen 
        ///</summary>
        public async Task<Person> CreateAsync(string name, IEnumerable<int> departmentIds)
        {
            var trimmed = (name ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed)) throw new PersonServiceException("Name darf nicht leer sein.");

            var deptIds = departmentIds.Distinct().ToList();
            if (deptIds.Count == 0) throw new PersonServiceException("Mindestens eine Abteilung muss zugeordnet sein.");

            var person = new Person { Name = trimmed, IsActive = true };
            _db.Persons.Add(person);

            foreach (var deptId in deptIds)
                _db.PersonDepartments.Add(new PersonDepartment
                {
                    Person = person,
                    DepartmentId = deptId
                });

            await _db.SaveChangesAsync();
            return person;
        }

        ///<summary>
        ///Benennt eine Person um
        ///</summary>
        public async Task RenameAsync(int id, string newName)
        {
            var trimmed = (newName ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed)) throw new PersonServiceException("Name darf nicht leer sein.");

            var person = await _db.Persons.FindAsync(id);
            if (person == null) throw new PersonServiceException("Person nicht gefunden.");

            person.Name = trimmed;
            await _db.SaveChangesAsync();
        }

        ///<summary>
        ///Setzt die Abteilkungs-Zurodnung einer Person neu
        ///Zuordnungen die nicht in Liste, werden entfernt; neue hinzugefügt
        ///</summary>
        public async Task UpdateDepartmentsAsync(int personId, IEnumerable<int> departmentIds)
        {
            var deptIds = departmentIds.Distinct().ToList();
            if (deptIds.Count == 0) throw new PersonServiceException("Mindestens eine Abteilung muss zugeordnet sein.");

            var person = await _db.Persons
                .Include(p => p.PersonDepartments)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null) throw new PersonServiceException("Person nicht gefunden");

            //Entferne was nicht mehr dabei ist
            var toRemove = person.PersonDepartments
                .Where(pd => !deptIds.Contains(pd.DepartmentId))
                .ToList();
            foreach (var pd in toRemove)
                _db.PersonDepartments.Remove(pd);

            //Neue Zuordnung
            var existing = person.PersonDepartments.Select(pd => pd.DepartmentId).ToHashSet();
            foreach (var deptId in deptIds.Where(id => !existing.Contains(id)))
                _db.PersonDepartments.Add(new PersonDepartment
                {
                    PersonId = personId,
                    DepartmentId = deptId
                });
            await _db.SaveChangesAsync();            
        }
        ///<summary>
        ///Soft-Delete: setzt IsActive = false. Person bleibt in DB
        ///erscheint aber nicht mehr in Defautl-Listen und bei neuen Termin
        ///</summary>
        public async Task DeactivateAsync(int id)
        {
            var person = await _db.Persons.FindAsync(id);
            if (person == null) throw new PersonServiceException("Person nicht gefunden.");

            person.IsActive = false;
            await _db.SaveChangesAsync();
        }

        ///<summary>Reaktiviert eine zuvor deaktivierte Person</summary>
        public async Task ReactivateAsync(int id)
        {
            var person = await _db.Persons.FindAsync(id);
            if (person == null) throw new PersonServiceException("Person nicht gefunden.");

            person.IsActive = true;
            await _db.SaveChangesAsync();
        }
    }
    public class PersonServiceException : Exception
    {
        public PersonServiceException(string message) : base(message) { }
    }
}
