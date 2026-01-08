using System.Windows;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Kursovva.Data;
using System.Windows.Controls;

namespace Kursovva
{
    public partial class EditTestWindow : Window
    {
        private int _examId;
        public EditTestWindow(int examId)
        {
            InitializeComponent();
            _examId = examId;
            LoadData();
        }
        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var exam = db.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == _examId);
                if (exam != null)
                {
                    txtTitle.Text = exam.Title;
                    QuestionsGrid.ItemsSource = exam.Questions.ToList();
                }
            }
        }
        private void BtnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            AddQuestionWindow win = new AddQuestionWindow(_examId);
            win.ShowDialog();
            LoadData();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            int qId = (int)((Button)sender).Tag;
            using (var db = new AppDbContext())
            {
                var q = db.Questions.Find(qId);
                if (q != null) { db.Questions.Remove(q); db.SaveChanges(); }
            }
            LoadData();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}