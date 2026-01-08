using System.Windows;
using System.Linq;
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class ProfileWindow : Window
    {
        private int _userId;

        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(_userId);
                if (user != null)
                {
                    txtFullName.Text = user.FullName;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(_userId);
                if (user == null) return;
                string oldPassInputHash = SecurityHelper.HashPassword(pbOldPass.Password);
                bool isOldPassCorrect = (user.Password == oldPassInputHash) || (user.Password == pbOldPass.Password);

                if (!isOldPassCorrect)
                {
                    MessageBox.Show("Введіть правильний старий пароль, щоб підтвердити зміни!",
                                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    user.FullName = txtFullName.Text;
                }
                if (!string.IsNullOrWhiteSpace(pbNewPass.Password))
                {
                    user.Password = SecurityHelper.HashPassword(pbNewPass.Password);
                }

                db.SaveChanges();
                MessageBox.Show("Профіль успішно оновлено!");
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}