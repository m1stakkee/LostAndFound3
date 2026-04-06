using LostAndFound3.Helpers;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class EditProfilePage : Page
    {
        private readonly string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=2;";

        private int _ownerId;

        public EditProfilePage()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT 
                    u.Id AS UserId,
                    u.Login,
                    o.Id AS OwnerId,
                    o.FullName,
                    o.Phone,
                    o.Email
                FROM Users u
                LEFT JOIN Owners o ON o.UserId = u.Id
                WHERE u.Id = @userId
            ", conn);

            cmd.Parameters.AddWithValue("@userId", SessionManager.CurrentUserId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                LoginBox.Text = reader["Login"]?.ToString() ?? "";
                FullNameBox.Text = reader["FullName"]?.ToString() ?? "";
                PhoneBox.Text = reader["Phone"]?.ToString() ?? "";
                EmailBox.Text = reader["Email"]?.ToString() ?? "";

                _ownerId = reader["OwnerId"] == DBNull.Value ? 0 : (int)reader["OwnerId"];
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string fullName = FullNameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var tr = conn.BeginTransaction();

            try
            {
                var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM Users
                    WHERE Login = @login AND Id <> @userId
                ", conn, tr);

                checkCmd.Parameters.AddWithValue("@login", login);
                checkCmd.Parameters.AddWithValue("@userId", SessionManager.CurrentUserId);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("Этот логин уже занят");
                    tr.Rollback();
                    return;
                }

                var updateUserCmd = new SqlCommand(@"
                    UPDATE Users
                    SET Login = @login
                    WHERE Id = @userId
                ", conn, tr);

                updateUserCmd.Parameters.AddWithValue("@login", login);
                updateUserCmd.Parameters.AddWithValue("@userId", SessionManager.CurrentUserId);
                updateUserCmd.ExecuteNonQuery();

                if (_ownerId == 0)
                {
                    var insertOwnerCmd = new SqlCommand(@"
                        INSERT INTO Owners (UserId, FullName, Phone, Email)
                        VALUES (@userId, @fullName, @phone, @email)
                    ", conn, tr);

                    insertOwnerCmd.Parameters.AddWithValue("@userId", SessionManager.CurrentUserId);
                    insertOwnerCmd.Parameters.AddWithValue("@fullName", fullName);
                    insertOwnerCmd.Parameters.AddWithValue("@phone", phone);
                    insertOwnerCmd.Parameters.AddWithValue("@email", email);
                    insertOwnerCmd.ExecuteNonQuery();
                }
                else
                {
                    var updateOwnerCmd = new SqlCommand(@"
                        UPDATE Owners
                        SET FullName = @fullName,
                            Phone = @phone,
                            Email = @email
                        WHERE Id = @ownerId
                    ", conn, tr);

                    updateOwnerCmd.Parameters.AddWithValue("@fullName", fullName);
                    updateOwnerCmd.Parameters.AddWithValue("@phone", phone);
                    updateOwnerCmd.Parameters.AddWithValue("@email", email);
                    updateOwnerCmd.Parameters.AddWithValue("@ownerId", _ownerId);
                    updateOwnerCmd.ExecuteNonQuery();
                }

                tr.Commit();

                SessionManager.CurrentLogin = login;

                MessageBox.Show("Профиль сохранён");
            }
            catch (Exception ex)
            {
                tr.Rollback();
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.MainFrameStatic != null && MainWindow.MainFrameStatic.CanGoBack)
                MainWindow.MainFrameStatic.GoBack();
            else if (UserWindow.UserFrameStatic != null && UserWindow.UserFrameStatic.CanGoBack)
                UserWindow.UserFrameStatic.GoBack();
        }
    }
}
