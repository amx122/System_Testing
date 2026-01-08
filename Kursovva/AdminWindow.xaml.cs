using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                UsersGrid.ItemsSource = db.Users.ToList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addWin = new AddUserWindow();
            addWin.ShowDialog();
            LoadUsers(); 
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            int userId = (int)((Button)sender).Tag;

            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    if (user.Username == "admin")
                    {
                        MessageBox.Show("Не можна видалити головного адміністратора!");
                        return;
                    }

                    if (MessageBox.Show($"Видалити користувача {user.Username}?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        db.Users.Remove(user);
                        db.SaveChanges();
                        LoadUsers();
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}