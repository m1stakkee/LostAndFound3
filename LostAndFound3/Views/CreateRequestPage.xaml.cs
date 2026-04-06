using LostAndFound.Helpers;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LostAndFound.Views
{
    public partial class CreateRequestPage : Page
    {
        private readonly string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public CreateRequestPage()
        {
            InitializeComponent();
        }

        private int? GetCurrentOwnerId()
        {
            if (string.IsNullOrWhiteSpace(SessionManager.CurrentLogin))
                return null;

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT o.Id
                FROM Users u
                INNER JOIN Owners o ON o.UserId = u.Id
                WHERE u.Login = @login", conn);

            cmd.Parameters.AddWithValue("@login", SessionManager.CurrentLogin);
            var result = cmd.ExecuteScalar();

            return result == null ? null : (int?)result;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string description = DescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || name == "Введите название вещи")
            {
                MessageBox.Show("Введите название вещи.");
                return;
            }

            if (string.IsNullOrWhiteSpace(description) || description == "Введите описание вещи")
            {
                MessageBox.Show("Введите описание вещи.");
                return;
            }

            var ownerId = GetCurrentOwnerId();
            if (!ownerId.HasValue)
            {
                MessageBox.Show("Не удалось определить пользователя.");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO Requests (OwnerId, Name, Description, Status)
                VALUES (@oid, @n, @d, 'Ожидает')", conn);

            cmd.Parameters.AddWithValue("@oid", ownerId.Value);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@d", description);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Заявка сохранена.");

            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "Введите название вещи")
            {
                NameBox.Text = "";
                NameBox.Foreground = Brushes.Black;
            }
        }

        private void NameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                NameBox.Text = "Введите название вещи";
                NameBox.Foreground = Brushes.Gray;
            }
        }

        private void DescriptionBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DescriptionBox.Text == "Введите описание вещи")
            {
                DescriptionBox.Text = "";
                DescriptionBox.Foreground = Brushes.Black;
            }
        }

        private void DescriptionBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DescriptionBox.Text))
            {
                DescriptionBox.Text = "Введите описание вещи";
                DescriptionBox.Foreground = Brushes.Gray;
            }
        }
    }
}
