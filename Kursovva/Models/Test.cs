using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Kursovva.Models
{
    public class Test
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
    }
}