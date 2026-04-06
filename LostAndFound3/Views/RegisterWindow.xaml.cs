using LostAndFound.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LostAndFound.Views
{
    public partial class RegisterWindow : Window
    {
        private PasswordHelper _passwordHasher = new PasswordHelper();

        private string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public RegisterWindow()
        {
            InitializeComponent();
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

        
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginBox.Text == "Введите логин" ? "" : LoginBox.Text;
                string password = PasswordBox.Password;

                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Введите логин");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите пароль");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Users WHERE Login=@login",
                        conn);

                    checkCmd.Parameters.AddWithValue("@login", login);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Логин уже занят");
                        return;
                    }

                    
                    string hash = _passwordHasher.HashPassword(password);

                    
                    var cmd = new SqlCommand(
                        "INSERT INTO Users (Login, PasswordHash, Role) VALUES (@login,@pass,'User')",
                        conn);

                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@pass", hash);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Регистрация успешна");

                    new LoginWindow().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}