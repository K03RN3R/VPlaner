using System;
using System.Collections.Generic;
using System.Text;
using VPlaner.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace VPlaner.Infrastructure.Data
{
    public static class DataSeeder
    {
        /// <summary>
        /// Standard-Abteilungen eines Karnevalsvereins. Werden nur beim
        /// allerersten Start angelegt (wenn Departments-Tabelle leer ist).
        /// </summary>

        private static readonly string[] DefaultDepartments =
            {
                "Allstars",
                "Showtanz",
                "Garde",
                "Stadtgarde",
                "Jugendgarde",
                "Kinder & Jugend",
                "Technik",
                "Vorstand",
                "Solos"
            };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Wenn Abteilung existiert, nichts zun
            if (await db.Departments.AnyAsync())
                return;

            foreach (var name in DefaultDepartments)
                db.Departments.Add(new Department { Name = name });

            await db.SaveChangesAsync();
        }
    }
}
