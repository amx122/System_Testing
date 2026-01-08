using System.Windows;
using System.Linq;
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class AddTestWindow : Window
    {
        private int _teacherId;
        public AddTestWindow(int teacherId)
        {
            InitializeComponent();
            _teacherId = teacherId;
            LoadSubjects();
        }
        private void LoadSubjects()
        {
            using (var db = new AppDbContext())
            {
                cmbSubject.ItemsSource = db.Subjects.ToList();
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbSubject.SelectedItem == null)
            {
                MessageBox.Show("Заповніть назву та предмет!"); return;
            }
            using (var db = new AppDbContext())
            {
                db.Exams.Add(new Exam
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    TimeLimitMinutes = (int)sliderTime.Value,
                    SubjectId = (int)cmbSubject.SelectedValue,
                    AuthorId = _teacherId
                });
                db.SaveChanges();
            }
            this.Close();
        }

        private void txtTitle_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}