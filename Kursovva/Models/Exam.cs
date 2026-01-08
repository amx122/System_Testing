using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public int TimeLimitMinutes { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        public int AuthorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}