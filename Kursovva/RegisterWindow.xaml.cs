using System.Windows;
using System.Windows.Input;
using System.Linq; 
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtRegLogin.Text.Trim();
            string name = txtRegName.Text.Trim();
            string pass = txtRegPass.Password.Trim();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == login))
                {
                    MessageBox.Show("Користувач з таким логіном вже існує. Оберіть інший.");
                    return;
                }

                var newUser = new User
                {
                    Username = login,
                    Password = SecurityHelper.HashPassword(pass), 
                    FullName = name,
                    Role = "Student"
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            MessageBox.Show("Реєстрація успішна! Тепер увійдіть у систему.");
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}