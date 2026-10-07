using System;
using System.Collections.Generic;
using System.Text;

namespace VPlaner.Core.Models
{

    /// <summary>
    /// Junction-Entity für die M:N-Beziehung zwischen Person und Department.
    /// Eine Person kann Mitglied in mehreren Abteilungen sein,
    /// eine Abteilung hat viele Mitglieder.
    /// </summary>
    public class PersonDepartment
    {
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        ///<summary> WAmm wurde die Person dieser Abteilung zugeordnet?</summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
