using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace VPlaner.Core.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public String Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Navigation Properties: alle Personen, die zu dieser Abteilung gehören
        public ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}
