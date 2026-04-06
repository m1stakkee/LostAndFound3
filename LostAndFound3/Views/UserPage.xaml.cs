using LostAndFound.Helpers;
using LostAndFound.ViewModels;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LostAndFound.Views
{
    public partial class UserPage : Page
    {
        private readonly MainViewModel vm;
        private readonly string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public UserPage()
        {
            InitializeComponent();
            vm = new MainViewModel();
            DataContext = vm;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Window.GetWindow(this)?.Close();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск...";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text == "Поиск...")
                return;

            vm.SearchText = SearchBox.Text;
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

        private void RequestSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedItem == null)
            {
                MessageBox.Show("Выберите вещь из списка.");
                return;
            }

            if (vm.SelectedItem.Status == "Выдано")
            {
                MessageBox.Show("Эта вещь уже выдана.");
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
                VALUES (@oid, @n, @d, 'Найдена')", conn);

            cmd.Parameters.AddWithValue("@oid", ownerId.Value);
            cmd.Parameters.AddWithValue("@n", vm.SelectedItem.Name ?? "");
            cmd.Parameters.AddWithValue("@d", vm.SelectedItem.Description ?? "");
            cmd.ExecuteNonQuery();

            MessageBox.Show("Заявка на выбранную вещь отправлена.");
        }

        private void CreateRequest_Click(object sender, RoutedEventArgs e)
        {
            UserWindow.UserFrameStatic.Navigate(new CreateRequestPage());
        }
    }
}
