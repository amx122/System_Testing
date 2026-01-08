using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Заповніть усі поля!");
                return;
            }

            string role = ((ComboBoxItem)cmbRole.SelectedItem).Tag.ToString();

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == txtUsername.Text))
                {
                    MessageBox.Show("Користувач з таким логіном вже існує!");
                    return;
                }

                var newUser = new User
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Text, 
                    FullName = txtFullName.Text,
                    Role = role
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            MessageBox.Show("Користувача успішно додано!");
            this.Close();
        }
    }
}