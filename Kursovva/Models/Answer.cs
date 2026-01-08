using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } 

        public bool IsCorrect { get; set; } 

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}