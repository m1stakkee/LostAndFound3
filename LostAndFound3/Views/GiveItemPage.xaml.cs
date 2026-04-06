using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class GiveItemPage : Page
    {
        private readonly string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private readonly int _itemId;
        private readonly int? _requestId;
        private readonly int? _preferredOwnerId;

        public GiveItemPage(int itemId, int? requestId = null, int? preferredOwnerId = null)
        {
            InitializeComponent();
            _itemId = itemId;
            _requestId = requestId;
            _preferredOwnerId = preferredOwnerId;
            LoadUsers();
        }

        public class Owner
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }

        private void LoadUsers()
        {
            var list = new List<Owner>();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT Id, FullName, Phone, Email FROM Owners", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Owner
                {
                    Id = (int)reader["Id"],
                    FullName = reader["FullName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString()
                });
            }

            UsersGrid.ItemsSource = list;

            if (_preferredOwnerId.HasValue)
            {
                foreach (var owner in list)
                {
                    if (owner.Id == _preferredOwnerId.Value)
                    {
                        UsersGrid.SelectedItem = owner;
                        break;
                    }
                }
            }
        }

        private void Give_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is not Owner owner)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var itemCmd = new SqlCommand(
                "UPDATE Items SET OwnerId=@oid, Status='Выдано' WHERE Id=@id", conn);
            itemCmd.Parameters.AddWithValue("@oid", owner.Id);
            itemCmd.Parameters.AddWithValue("@id", _itemId);
            itemCmd.ExecuteNonQuery();

            if (_requestId.HasValue)
            {
                var requestCmd = new SqlCommand(
                    "UPDATE Requests SET Status='Выдано' WHERE Id=@rid", conn);
                requestCmd.Parameters.AddWithValue("@rid", _requestId.Value);
                requestCmd.ExecuteNonQuery();
            }

            MessageBox.Show("Вещь выдана!");
            MainWindow.MainFrameStatic.Navigate(new MainPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
