using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VPlaner.Core.Models
{
    public class Person
    {
        public int Id { get; set; }

        //Fremdschlüssel und Navigation zur Abteilung
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Vorname { get; set; } = string.Empty;

        [Required, MaxLength(1)]
        public string Nachname { get; set; } = string.Empty;

        /// <summary>
        /// Soft-Delete-Flag. wenn eine Person austritt ist sie in der
        /// Veranstaltungshistorie noch vorhanden, aber nicht mehr aktiv.
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
