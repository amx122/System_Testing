using Kursovva.Data;
using Kursovva.Models;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Kursovva
{
    public partial class ReviewWindow : Window
    {
        private int _testResultId;

        public ReviewWindow(int testResultId)
        {
            InitializeComponent();
            _testResultId = testResultId;
            LoadReview();
        }

        private void LoadReview()
        {
            using (var db = new AppDbContext())
            {
                var result = db.TestResults
                    .Include(tr => tr.UserAnswers)
                        .ThenInclude(ua => ua.Question)
                            .ThenInclude(q => q.Answers)
                    .FirstOrDefault(r => r.Id == _testResultId);

                if (result == null) return;

                var reviewList = new List<ReviewItem>();

                foreach (var ua in result.UserAnswers)
                {
                    var question = ua.Question;
                    var userAnswer = question.Answers.FirstOrDefault(a => a.Id == ua.SelectedAnswerId);
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

                    bool isCorrect = (userAnswer?.Id == correctAnswer?.Id);

                    reviewList.Add(new ReviewItem
                    {
                        QuestionText = question.Text,
                        UserAnswerText = userAnswer != null ? userAnswer.Text : "Немає відповіді",
                        CorrectAnswerText = correctAnswer != null ? correctAnswer.Text : "",

                        StatusText = isCorrect ? "✅ Вірно" : "❌ Помилка",
                        StatusColor = isCorrect ? Brushes.Green : Brushes.Red,
                        UserAnswerColor = isCorrect ? Brushes.Green : Brushes.Red,
                        ShowCorrectAnswer = isCorrect ? Visibility.Collapsed : Visibility.Visible
                    });
                }

                ListQuestions.ItemsSource = reviewList;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
    public class ReviewItem
    {
        public string QuestionText { get; set; }
        public string UserAnswerText { get; set; }
        public string CorrectAnswerText { get; set; }
        public string StatusText { get; set; }
        public Brush StatusColor { get; set; }
        public Brush UserAnswerColor { get; set; }
        public Visibility ShowCorrectAnswer { get; set; }
    }
}