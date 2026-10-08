using System;
using System.Collections.Generic;
using System.Text;
using VPlaner.Core.Models;
using VPlaner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace VPlaner.Infrastructure.Services

/// <summary>
/// Geschäftslogik rund um Abteilungen (CRUD).
/// Hält die Component frei von DB-Zugriff und zentralisiert
/// Validierung sowie Fehler-Übersetzung.
/// </summary>
{
    public class DepartmentService
    {
        private readonly AppDbContext _db;

        public DepartmentService(AppDbContext db) => _db = db;
        
        ///<summary>Alle Abteilungen aufsteigend nach ID sortiert</summary>
        public Task <List<Department>> GetAllAsync()
            => _db.Departments
                .OrderBy(d => d.Id)
                .ToListAsync();

        public Task<Department?> GetAsync(int id)
            => _db.Departments.FirstOrDefaultAsync(d => d.Id == id);

        /// <summary>Legt eine neue Abteilung an. Wirft <see cref="DepartmentServiceException"/> bei Duplikat.</summary>
        public async Task<Department> CreateAsync(string name)
        {
            var trimmed = (name ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
                throw new DepartmentServiceException("Name darf nicht leer sein.");

            var dept = new Department { Name = trimmed };
            _db.Departments.Add(dept);

            try
            {
                await _db.SaveChangesAsync();
                return dept;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _db.ChangeTracker.Clear();
                throw new DepartmentServiceException($"Eine Abteilung mit dem Namen '{trimmed}' existiert bereits.");
            }
        }

        /// <summary>Benennt eine Abteilung um. Wirft <see cref="DepartmentServiceException"/> bei Duplikat oder leerem Namen.</summary>
        public async Task RenameAsync(int id, string newName)
        {
            var trimmed = (newName ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
                throw new DepartmentServiceException("Name darf nicht leer sein.");

            var dept = await _db.Departments.FindAsync(id);
            if (dept == null)
                throw new DepartmentServiceException("Abteilung nicht gefunden.");

            dept.Name = trimmed;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _db.ChangeTracker.Clear();
                throw new DepartmentServiceException($"Eine Abteilung mit dem Namen '{trimmed}' existiert bereits.");
            }
        }

        /// <summary>Löscht eine Abteilung. Wirft <see cref="DepartmentServiceException"/>, wenn noch Verknüpfungen existieren.</summary>
        public async Task DeleteAsync(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null)
                throw new DepartmentServiceException("Abteilung nicht gefunden.");

            _db.Departments.Remove(dept);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _db.ChangeTracker.Clear();
                throw new DepartmentServiceException(
                    $"'{dept.Name}' kann nicht gelöscht werden — es sind noch Personen oder Termine damit verknüpft.");
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
            => ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// Fachliche Exception, die der Service wirft. Ihre Message ist für den
    /// Nutzer gedacht und kann direkt angezeigt werden.
    /// </summary>
    public class DepartmentServiceException : Exception
    {
        public DepartmentServiceException(string message) : base(message) { }
    }
}

