using System;
using System.Collections.Generic;
using System.Text;
using VPlaner.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace VPlaner.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets - eine Property pro Entity Typ
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Person> Persons => Set<Person>();
        public DbSet<PersonDepartment> PersonDepartments => Set<PersonDepartment>();
        public DbSet<EventItem> Events => Set<EventItem>();
        public DbSet<EventDepartment> EventDepartments => Set<EventDepartment>();
        public DbSet<EventAttendance> Attendances => Set<EventAttendance>();

        protected override void OnModelCreating (ModelBuilder b)


        {
            base.OnModelCreating(b);

            // Department
            b.Entity<Department>(e =>
            {
                e.HasIndex(d => d.Name).IsUnique();
            });

            //Person
            b.Entity<PersonDepartment>(e =>
            {
                e.HasKey(pd => new { pd.PersonId, pd.DepartmentId });

                e.HasOne(pd => pd.Person)
                .WithMany(p => p.PersonDepartments)
                .HasForeignKey(pd => pd.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pd => pd.Department)
                .WithMany(d => d.PersonDepartments)
                .HasForeignKey(pd => pd.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // EventItem
            b.Entity<EventItem>(e =>
            {
                e.HasIndex(ev => ev.ImportKey).IsUnique();
            });

            // EventDepartment (Junction für M:N Event/Department)
            b.Entity<EventDepartment>(e =>
            {
                e.HasKey(ed => new { ed.EventId, ed.DepartmentId });

                e.HasOne(ed => ed.Event)
                 .WithMany(ev => ev.EventDepartments)
                 .HasForeignKey(ed => ed.EventId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ed => ed.Department)
                 .WithMany(d => d.EventDepartments)
                 .HasForeignKey(ed => ed.DepartmentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // EventAttendance
            b.Entity<EventAttendance>(e =>
                {
                    e.HasOne(a => a.Event)
                    .WithMany(ev => ev.Attendances)
                    .HasForeignKey(a => a.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(a => a.Person)
                     .WithMany()
                     .HasForeignKey(a => a.PersonId)
                     .OnDelete(DeleteBehavior.Restrict);

                    e.HasIndex(a => new { a.EventId, a.PersonId }).IsUnique();
                });
        }
    }
}
