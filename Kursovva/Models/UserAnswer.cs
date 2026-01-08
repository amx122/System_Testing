using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }

        public int TestResultId { get; set; }
        public virtual TestResult TestResult { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }

        public int SelectedAnswerId { get; set; } 
    }
}