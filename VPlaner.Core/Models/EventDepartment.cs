using System;
using System.Collections.Generic;
using System.Text;

namespace VPlaner.Core.Models
{
    /// <summary>
    /// Junction-Entity für die M:N-Beziehung zwischen EventItem und Department.
    /// Ein Termin betrifft 1..n Abteilungen (aus der Auftritt-Spalte des PDF).
    /// </summary>
    public class EventDepartment
    {
        public int EventId { get; set; }
        public EventItem Event { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
