using System.Windows;
using System.Windows.Input; 
using System.Linq;
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, введіть логін та пароль.");
                return;
            }

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                if (!db.Users.Any())
                {
                    var admin = new Kursovva.Models.User
                    {
                        Username = "admin",
                        Password = SecurityHelper.HashPassword("admin"),
                        FullName = "System Admin",
                        Role = "Admin"
                    };
                    db.Users.Add(admin);
                    db.SaveChanges();
                }

                string passwordHash = SecurityHelper.HashPassword(password);
                var user = db.Users.FirstOrDefault(u => u.Username == login && u.Password == passwordHash);
                if (user == null)
                {
                    user = db.Users.FirstOrDefault(u => u.Username == login && u.Password == password);
                }

                if (user != null)
                {
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль!", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}