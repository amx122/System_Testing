using System; 
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using Kursovva.Models;
using Kursovva.Data;

namespace Kursovva
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        public Visibility IsTeacherVisible { get; set; } = Visibility.Collapsed;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.Role == "Admin" || _currentUser.Role == "Teacher")
                IsTeacherVisible = Visibility.Visible;
            else
                IsTeacherVisible = Visibility.Collapsed;

            DataContext = this;

            LoadUserData();
            EnsureTestData();
            Menu_Home_Click(null, null);
        }

        private void Menu_Home_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Visible;
            TestsView.Visibility = Visibility.Collapsed;
            ResultsView.Visibility = Visibility.Collapsed;
            LoadDashboard();
        }

        private void Menu_Tests_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            TestsView.Visibility = Visibility.Visible;
            ResultsView.Visibility = Visibility.Collapsed;
            LoadTests();
        }

        private void Menu_Results_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            TestsView.Visibility = Visibility.Collapsed;
            ResultsView.Visibility = Visibility.Visible;
            LoadResults();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (DashboardView.Visibility == Visibility.Visible) LoadDashboard();
            else if (TestsView.Visibility == Visibility.Visible) LoadTests();
            else LoadResults();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWin = new ProfileWindow(_currentUser.Id);
            profileWin.ShowDialog();
            using (var db = new AppDbContext())
            {
                var u = db.Users.Find(_currentUser.Id);
                if (u != null) { _currentUser = u; txtUserName.Text = u.FullName; }
            }
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                txtUserName.Text = _currentUser.FullName;
                txtUserRole.Text = _currentUser.Role switch
                {
                    "Admin" => "Адміністратор",
                    "Teacher" => "Викладач",
                    _ => "Студент"
                };

                if (BtnAdminMenu != null)
                    BtnAdminMenu.Visibility = (_currentUser.Role == "Admin") ? Visibility.Visible : Visibility.Collapsed;

                if (BtnCreateTest != null)
                    BtnCreateTest.Visibility = (_currentUser.Role == "Admin" || _currentUser.Role == "Teacher")
                                               ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void LoadDashboard()
        {
            using (var db = new AppDbContext())
            {
                txtWelcome.Text = $"З поверненням, {_currentUser.FullName}!";

                if (_currentUser.Role == "Student")
                {
                    int passedCount = db.TestResults.Count(r => r.UserId == _currentUser.Id);
                    lblStat1Value.Text = passedCount.ToString();
                    lblStat1Title.Text = "Пройдено тестів";

                    var myResults = db.TestResults.Where(r => r.UserId == _currentUser.Id).ToList();
                    double avgScore = myResults.Any() ? myResults.Average(r => r.Percentage) : 0;
                    lblStat2Value.Text = $"{Math.Round(avgScore)}%";
                    lblStat2Title.Text = "Середня успішність";

                    int totalExams = db.Exams.Count();
                    lblStat3Value.Text = totalExams.ToString();
                    lblStat3Title.Text = "Доступно тестів";
                }
                else
                {
                    int examCount = db.Exams.Count();
                    lblStat1Value.Text = examCount.ToString();
                    lblStat1Title.Text = "Створено тестів";

                    int studentCount = db.Users.Count(u => u.Role == "Student");
                    lblStat2Value.Text = studentCount.ToString();
                    lblStat2Title.Text = "Кількість студентів";

                    int totalAttempts = db.TestResults.Count();
                    lblStat3Value.Text = totalAttempts.ToString();
                    lblStat3Title.Text = "Всього проходжень";
                }
            }
        }

        private void LoadTests()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Exams.AsQueryable();

                if (txtSearchTests != null && !string.IsNullOrWhiteSpace(txtSearchTests.Text))
                {
                    string search = txtSearchTests.Text.ToLower();
                    query = query.Where(e => e.Title.ToLower().Contains(search) || e.Description.ToLower().Contains(search));
                }

                var exams = query.ToList();
                var displayList = exams.Select((e, index) => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.TimeLimitMinutes,
                    ColorHex = index % 2 == 0 ? "#673AB7" : "#3F51B5"
                }).ToList();

                TestsList.ItemsSource = displayList;
            }
        }

        private void LoadResults()
        {
            using (var db = new AppDbContext())
            {
                var query = db.TestResults.Include(r => r.Exam).Include(r => r.User).AsQueryable();

                if (_currentUser.Role != "Admin" && _currentUser.Role != "Teacher")
                    query = query.Where(r => r.UserId == _currentUser.Id);

                if (txtSearchStudent != null && !string.IsNullOrWhiteSpace(txtSearchStudent.Text))
                {
                    string search = txtSearchStudent.Text.ToLower();
                    query = query.Where(r => r.User.FullName.ToLower().Contains(search) || r.Exam.Title.ToLower().Contains(search));
                }

                var results = query.OrderByDescending(r => r.DateTaken).ToList();
                var gridData = results.Select(r => new
                {
                    r.Id,  
                    StudentName = r.User.FullName,
                    ExamName = r.Exam.Title,
                    r.DateTaken,
                    ScoreText = $"{r.Score} з {r.MaxScore}",
                    r.Percentage,
                    IsPassed = r.Percentage >= 60
                }).ToList();

                ResultsGrid.ItemsSource = gridData;

                if (ResultsGrid.Columns.Count > 0)
                {
                    if (_currentUser.Role == "Student") ResultsGrid.Columns[0].Visibility = Visibility.Collapsed;
                    else ResultsGrid.Columns[0].Visibility = Visibility.Visible;
                }
            }
        }

        private void TxtSearchTests_TextChanged(object sender, TextChangedEventArgs e) => LoadTests();
        private void TxtSearchResults_TextChanged(object sender, TextChangedEventArgs e) => LoadResults();

        private void BtnStartTest_Click(object sender, RoutedEventArgs e)
        {
            int examId = (int)((Button)sender).Tag;
            TestWindow testWindow = new TestWindow(examId, _currentUser.Id);
            testWindow.ShowDialog();
            if (ResultsView.Visibility == Visibility.Visible) LoadResults();
            if (DashboardView.Visibility == Visibility.Visible) LoadDashboard();
        }

        private void BtnCreateTest_Click(object sender, RoutedEventArgs e)
        {
            AddTestWindow addWindow = new AddTestWindow(_currentUser.Id);
            addWindow.ShowDialog();
            LoadTests();
            LoadDashboard();
        }

        private void BtnEditTest_Click(object sender, RoutedEventArgs e)
        {
            int examId = (int)((Button)sender).Tag;
            EditTestWindow editWindow = new EditTestWindow(examId);
            editWindow.ShowDialog();
            LoadTests();
        }

        private void BtnDeleteTest_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Видалити цей тест?", "Видалення", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int examId = (int)((Button)sender).Tag;
                using (var db = new AppDbContext())
                {
                    var exam = db.Exams.Find(examId);
                    if (exam != null) { db.Exams.Remove(exam); db.SaveChanges(); }
                }
                LoadTests();
                LoadDashboard();
            }
        }

        private void BtnAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWin = new AdminWindow();
            adminWin.ShowDialog();
            BtnAdminMenu.IsChecked = false;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var data = ResultsGrid.ItemsSource;
            if (data == null) return;
            SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON|*.json", FileName = "Report" };
            if (sfd.ShowDialog() == true)
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(sfd.FileName, JsonSerializer.Serialize(data, opts));
                MessageBox.Show("Збережено!");
            }
        }
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWin = new AboutWindow();
            aboutWin.ShowDialog();
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = ResultsGrid.SelectedItem;

            if (selectedItem != null)
            {
                int resultId = selectedItem.Id;
                ReviewWindow reviewWin = new ReviewWindow(resultId);
                reviewWin.ShowDialog();
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть рядок у таблиці, щоб переглянути деталі.",
                                "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void EnsureTestData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    if (!db.Users.Any(u => u.Username == "admin"))
                    {
                        var admin = new User
                        {
                            Username = "admin",
                            Password = SecurityHelper.HashPassword("admin"), 
                            FullName = "Головний Адміністратор",
                            Role = "Admin"
                        };
                        db.Users.Add(admin);
                        db.SaveChanges();
                    }

                    if (!db.Subjects.Any())
                    {
                        var subjectCS = new Subject { Name = "C#", Description = "Programming" };
                        db.Subjects.Add(subjectCS);
                        db.SaveChanges();

                        var exam = new Exam { Title = "C# Basics", SubjectId = subjectCS.Id, TimeLimitMinutes = 15, Description = "Base types", AuthorId = 1 };
                        db.Exams.Add(exam);
                        db.SaveChanges();

                        var q = new Question { Text = "Value of int?", Type = "Single", ExamId = exam.Id };
                        db.Questions.Add(q);
                        db.SaveChanges();

                        db.Answers.Add(new Answer { Text = "32 bit", IsCorrect = true, QuestionId = q.Id });
                        db.Answers.Add(new Answer { Text = "16 bit", IsCorrect = false, QuestionId = q.Id });
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }
}