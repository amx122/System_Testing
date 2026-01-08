using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;

namespace Kursovva.Models
{
    public class TestResult
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;

        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}