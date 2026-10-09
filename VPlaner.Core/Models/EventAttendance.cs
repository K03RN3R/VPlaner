using System;
using System.Collections.Generic;
using System.Text;

namespace VPlaner.Core.Models
{

    ///<summary>
    ///Anwesenheitsstatus einer Person für einen Termin
    ///Pro (EventId, PersonId) gibt es maximal einen Eintrag (Unique Constraint)
    ///</summary>
    public class EventAttendance
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public EventItem Event { get; set; } = null!;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Ausstehend;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        ///<summary> User-Identität, die den Status geändert hat</summary>
        public string? UpdatedBy { get; set; }
    }
}
