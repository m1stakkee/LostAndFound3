using LostAndFound3.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LostAndFound.Views
{
    public partial class LoginWindow : Window
    {
        private PasswordHelper _passwordHasher = new PasswordHelper();

        private string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public LoginWindow()
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

        
        private void PsbPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PsbPass.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        
        private void PsbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }

       
        
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginBox.Text == "Введите логин" ? "" : LoginBox.Text;
                string password = PsbPass.Password;

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

                    var cmd = new SqlCommand(
                        "SELECT PasswordHash, Role FROM Users WHERE Login=@login",
                        conn);

                    cmd.Parameters.AddWithValue("@login", login);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string dbHash = reader["PasswordHash"].ToString();
                        string role = reader["Role"].ToString();

                        if (_passwordHasher.VerifyPassword(password, dbHash))
                        {
                            MessageBox.Show("Успешный вход");

                            if (role == "Admin")
                                new MainWindow().Show();
                            else
                                new UserWindow().Show();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неверный пароль");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();

            this.Close();
        }
    }
}