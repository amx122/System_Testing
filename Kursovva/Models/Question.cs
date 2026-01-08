using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public byte[]? ImageData { get; set; }

        public int Points { get; set; } = 1;

        public string Type { get; set; } 

        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}