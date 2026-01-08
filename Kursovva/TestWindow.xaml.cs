using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore; 
using Kursovva.Models;
using Kursovva.Data; 

namespace Kursovva
{
    public partial class TestWindow : Window
    {
        private int _examId;
        private int _userId;
        private Exam _currentExam;     
        private User _currentUser;     
        private List<Question> _questions;
        private int _currentQuestionIndex = 0;
        private DispatcherTimer _timer;
        private int _timeLeftSeconds;
        private Dictionary<int, int> _studentSelections = new Dictionary<int, int>();

        public TestWindow(int examId, int userId)
        {
            InitializeComponent();
            _examId = examId;
            _userId = userId;

            LoadTest();
        }

        private void LoadTest()
        {
            using (var db = new AppDbContext())
            {
                _currentExam = db.Exams
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers)
                    .FirstOrDefault(e => e.Id == _examId);

                _currentUser = db.Users.Find(_userId);

                if (_currentExam == null || _currentUser == null)
                {
                    MessageBox.Show("Помилка завантаження тесту.");
                    Close();
                    return;
                }

                _questions = _currentExam.Questions.ToList();
                _timeLeftSeconds = _currentExam.TimeLimitMinutes * 60;
                StartTimer();
                ShowQuestion(_currentQuestionIndex);
            }
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeLeftSeconds--;
            TimeSpan t = TimeSpan.FromSeconds(_timeLeftSeconds);
            txtTimer.Text = t.ToString(@"mm\:ss");
            if (_timeLeftSeconds <= 60) txtTimer.Foreground = Brushes.Red;
            else txtTimer.Foreground = Brushes.Black;

            if (_timeLeftSeconds <= 0)
            {
                FinishTest();
            }
        }

        private void ShowQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count) return;

            var q = _questions[index];

            txtQuestionText.Text = $"{index + 1}. {q.Text}";
            AnswersPanel.Children.Clear();
            foreach (var ans in q.Answers)
            {
                var rb = new RadioButton
                {
                    Content = ans.Text,
                    FontSize = 16,
                    Margin = new Thickness(0, 5, 0, 5),
                    Tag = ans.Id 
                };
                if (_studentSelections.ContainsKey(q.Id) && _studentSelections[q.Id] == ans.Id)
                {
                    rb.IsChecked = true;
                }
                rb.Checked += (s, e) =>
                {
                    _studentSelections[q.Id] = ans.Id;
                };

                AnswersPanel.Children.Add(rb);
            }
            BtnPrev.IsEnabled = index > 0;
            BtnNext.Content = (index == _questions.Count - 1) ? "ЗАВЕРШИТИ" : "ДАЛІ";
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                ShowQuestion(_currentQuestionIndex);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                ShowQuestion(_currentQuestionIndex);
            }
            else
            {
                if (MessageBox.Show("Ви впевнені, що хочете завершити тест?", "Завершення", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    FinishTest();
                }
            }
        }

        private void FinishTest()
        {
            _timer.Stop();

            int score = 0;
            int maxScore = 0;
            var userAnswersToSave = new List<UserAnswer>();

            foreach (var q in _questions)
            {
                var correctParams = q.Answers.FirstOrDefault(a => a.IsCorrect);
                if (correctParams != null) maxScore += q.Points > 0 ? q.Points : 1; 

                if (_studentSelections.ContainsKey(q.Id))
                {
                    int selectedId = _studentSelections[q.Id];

                    if (correctParams != null && correctParams.Id == selectedId)
                    {
                        score += q.Points > 0 ? q.Points : 1;
                    }
                    userAnswersToSave.Add(new UserAnswer
                    {
                        QuestionId = q.Id,
                        SelectedAnswerId = selectedId
                    });
                }
            }
            using (var db = new AppDbContext())
            {
                var result = new TestResult
                {
                    UserId = _currentUser.Id,
                    ExamId = _currentExam.Id,
                    Score = score,
                    MaxScore = maxScore,
                    DateTaken = DateTime.Now,
                    Percentage = maxScore > 0 ? (double)score / maxScore * 100 : 0
                };

                db.TestResults.Add(result);
                db.SaveChanges(); 
                foreach (var ua in userAnswersToSave)
                {
                    ua.TestResultId = result.Id;
                    db.UserAnswers.Add(ua);
                }
                db.SaveChanges();
            }

            MessageBox.Show($"Тест завершено!\nВаш результат: {score} з {maxScore}");
            Close();
        }
    }
}