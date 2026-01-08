using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}