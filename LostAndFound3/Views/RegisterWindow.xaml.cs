using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace LostAndFound.Views
{
    public partial class RegisterWindow : Window
    {
        private string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private PasswordHelper _hasher = new PasswordHelper();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;
            string name = NameBox.Text.Trim();
            string surname = SurnameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();

            
            if (string.IsNullOrWhiteSpace(login) || login == "Введите логин")
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) || name == "Введите имя")
            {
                MessageBox.Show("Введите имя");
                return;
            }

            if (string.IsNullOrWhiteSpace(surname) || surname == "Введите фамилию")
            {
                MessageBox.Show("Введите фамилию");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone) || phone == "Введите телефон")
            {
                MessageBox.Show("Введите телефон");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || email == "Введите почту")
            {
                MessageBox.Show("Введите почту");
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                
                var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Login=@l", conn);

                checkCmd.Parameters.AddWithValue("@l", login);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("Такой логин уже существует!");
                    return;
                }

                
                string hash = _hasher.HashPassword(password);

                
                var userCmd = new SqlCommand(
                    "INSERT INTO Users (Login, PasswordHash, Role) OUTPUT INSERTED.Id VALUES (@l,@p,'User')",
                    conn);

                userCmd.Parameters.AddWithValue("@l", login);
                userCmd.Parameters.AddWithValue("@p", hash);

                int userId = (int)userCmd.ExecuteScalar();

                
                var ownerCmd = new SqlCommand(
                    "INSERT INTO Owners (UserId,FullName, Phone, Email) VALUES (@uid,@n,@ph,@em)",
                    conn);

                ownerCmd.Parameters.AddWithValue("@uid", userId);
                ownerCmd.Parameters.AddWithValue("@n", name + " " + surname);
                ownerCmd.Parameters.AddWithValue("@ph", phone);
                ownerCmd.Parameters.AddWithValue("@em", email);

                ownerCmd.ExecuteNonQuery();
            }

            MessageBox.Show("Регистрация успешна!");

            new LoginWindow().Show();
            this.Close();
        }

        

        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "Введите логин")
            {
                LoginBox.Text = "";
                LoginBox.Foreground = Brushes.Black;
            }
        }

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                LoginBox.Text = "Введите логин";
                LoginBox.Foreground = Brushes.Gray;
            }
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "Введите имя")
            {
                NameBox.Text = "";
                NameBox.Foreground = Brushes.Black;
            }
        }

        private void NameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                NameBox.Text = "Введите имя";
                NameBox.Foreground = Brushes.Gray;
            }
        }

        private void SurnameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SurnameBox.Text == "Введите фамилию")
            {
                SurnameBox.Text = "";
                SurnameBox.Foreground = Brushes.Black;
            }
        }

        private void SurnameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SurnameBox.Text))
            {
                SurnameBox.Text = "Введите фамилию";
                SurnameBox.Foreground = Brushes.Gray;
            }
        }

        private void PhoneBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PhoneBox.Text == "Введите телефон")
            {
                PhoneBox.Text = "";
                PhoneBox.Foreground = Brushes.Black;
            }
        }

        private void PhoneBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                PhoneBox.Text = "Введите телефон";
                PhoneBox.Foreground = Brushes.Gray;
            }
        }

        private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailBox.Text == "Введите почту")
            {
                EmailBox.Text = "";
                EmailBox.Foreground = Brushes.Black;
            }
        }

        private void EmailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                EmailBox.Text = "Введите почту";
                EmailBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}