using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VPlaner.Core.Models
{
    public class EventItem
    {
        public int Id { get; set; }

        /// <summary>Start-Datum. Null bedeutet "noch nicht festgelegt" (PDF "?").</summary>
        public DateTime? Date { get; set; }

        /// <summary>Optionales End-Datum für Mehrtagestermine (z. B. "25. - 27. Sep.").</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Startzeit. Null wenn im PDF "?" steht.</summary>
        public TimeSpan? Time { get; set; }

        /// <summary>Treffen-Zeit (Aufruf vor dem Termin). Optional.</summary>
        public TimeSpan? MeetingTime { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;         // Spalte "Veranstaltung"

        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;      // Spalte "Adresse"

        /// <summary>Flag aus Auftritt-Spalte "bestl. Karten" — nur Personen mit Karten kommen.</summary>
        public bool RequiresTickets { get; set; }

        /// <summary>Original-Auftritt-Text und nicht erkannte Tokens, Mehrtages-Notizen etc.</summary>
        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Künstlicher Schlüssel zur Duplikat-Erkennung beim PDF-Re-Import.
        /// Setzt sich aus Datum + Zeit + Title + Location zusammen.
        /// </summary>
        [Required, MaxLength(500)]
        public string ImportKey { get; set; } = string.Empty;

        // Navigation: M:N zu Department über Junction
        public ICollection<EventDepartment> EventDepartments { get; set; } = new List<EventDepartment>();

        // Navigation: 1:N — alle Anwesenheiten zu diesem Termin
        public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();

        /// <summary>
        /// Termin liegt komplett in der Vergangenheit → gesperrt für Änderungen.
        /// Ohne Datum (TBD) bleibt er offen und veränderbar.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                if (Date == null) return false;
                var effectiveEnd = (EndDate ?? Date.Value).Date.AddDays(1);
                return effectiveEnd < DateTime.Now.Date.AddDays(1);
            }
        }

        /// <summary>Formatierter Datumstext für die UI (auch Bereiche und TBD).</summary>
        public string DateLabel
        {
            get
            {
                if (Date == null) return "Datum offen";
                if (EndDate.HasValue && EndDate.Value.Date != Date.Value.Date)
                    return $"{Date:dd.MM.yyyy} – {EndDate:dd.MM.yyyy}";
                return Date.Value.ToString("dd.MM.yyyy");
            }
        }
    }
}
