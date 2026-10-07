using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VPlaner.Core.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Soft-Delete-Flag. wenn eine Person austritt ist sie in der
        /// Veranstaltungshistorie noch vorhanden, aber nicht mehr aktiv.
        /// Dadurch keine Lücken in der Historie, aber die Person wird nicht mehr angezeigt.
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Abteilunge, in denen diese Person Mitglied ist (M:N)
        /// </summary>
        public ICollection<PersonDepartment> PersonDepartments { get; set; } = new List<PersonDepartment>();
    }
}
