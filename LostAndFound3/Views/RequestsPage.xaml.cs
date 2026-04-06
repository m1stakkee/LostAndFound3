using LostAndFound.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class RequestsPage : Page
    {
        private readonly string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private ObservableCollection<Request> Requests { get; } = new();

        public RequestsPage()
        {
            InitializeComponent();
            LoadRequests();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void LoadRequests()
        {
            Requests.Clear();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT r.Id, r.OwnerId, r.Name, r.Description, r.Status,
                       o.FullName, o.Phone, o.Email
                FROM Requests r
                INNER JOIN Owners o ON r.OwnerId = o.Id
                ORDER BY r.Id DESC", conn);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Requests.Add(new Request
                {
                    Id = (int)reader["Id"],
                    OwnerId = (int)reader["OwnerId"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString(),
                    OwnerFullName = reader["FullName"].ToString(),
                    OwnerPhone = reader["Phone"].ToString(),
                    OwnerEmail = reader["Email"].ToString()
                });
            }

            RequestsGrid.ItemsSource = Requests;
        }

        private Request? GetSelectedRequest()
        {
            return RequestsGrid.SelectedItem as Request;
        }

        private void FoundItem_Click(object sender, RoutedEventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null)
            {
                MessageBox.Show("Выберите заявку.");
                return;
            }

            if (request.Status == "Найдена" || request.Status == "Выдано")
            {
                MessageBox.Show("Для этой заявки вещь уже отмечена как найденная.");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var insertItemCmd = new SqlCommand(@"
                INSERT INTO Items (Name, Description, Status)
                VALUES (@n, @d, 'Найдено')", conn);

            insertItemCmd.Parameters.AddWithValue("@n", request.Name ?? "");
            insertItemCmd.Parameters.AddWithValue("@d", request.Description ?? "");
            insertItemCmd.ExecuteNonQuery();

            var updateRequestCmd = new SqlCommand(@"
                UPDATE Requests SET Status='Найдена' WHERE Id=@id", conn);

            updateRequestCmd.Parameters.AddWithValue("@id", request.Id);
            updateRequestCmd.ExecuteNonQuery();

            MessageBox.Show("Вещь добавлена в основной список.");
            LoadRequests();
            MainWindow.MainFrameStatic.Navigate(new MainPage());
        }

        private void IssueFromRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null)
            {
                MessageBox.Show("Выберите заявку.");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var findItemCmd = new SqlCommand(@"
                SELECT TOP 1 Id
                FROM Items
                WHERE Name=@n
                  AND ISNULL(Description,'') = ISNULL(@d,'')
                  AND Status='Найдено'
                ORDER BY Id DESC", conn);

            findItemCmd.Parameters.AddWithValue("@n", request.Name ?? "");
            findItemCmd.Parameters.AddWithValue("@d", request.Description ?? "");

            var result = findItemCmd.ExecuteScalar();

            if (result == null)
            {
                MessageBox.Show("Подходящая найденная вещь не найдена. Сначала нажмите 'Нашли вещь'.");
                return;
            }

            int itemId = (int)result;

            var issueCmd = new SqlCommand(@"
                UPDATE Items
                SET OwnerId=@ownerId, Status='Выдано'
                WHERE Id=@itemId", conn);

            issueCmd.Parameters.AddWithValue("@ownerId", request.OwnerId);
            issueCmd.Parameters.AddWithValue("@itemId", itemId);
            issueCmd.ExecuteNonQuery();

            var requestCmd = new SqlCommand(@"
                UPDATE Requests
                SET Status='Выдано'
                WHERE Id=@rid", conn);

            requestCmd.Parameters.AddWithValue("@rid", request.Id);
            requestCmd.ExecuteNonQuery();

            MessageBox.Show("Вещь выдана по заявке.");
            LoadRequests();
            MainWindow.MainFrameStatic.Navigate(new MainPage());
        }
    }
}
