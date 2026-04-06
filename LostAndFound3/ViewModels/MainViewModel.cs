using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LostAndFound.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace LostAndFound.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=2;";

        [ObservableProperty]
        private ObservableCollection<Item> items = new();

        [ObservableProperty]
        private Item selectedItem;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string searchText;

        public MainViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load(string search = "")
        {
            Items.Clear();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT 
                    i.Id, 
                    i.Name, 
                    i.Description, 
                    i.Status,
                    i.OwnerId,
                    o.FullName,
                    o.Phone,
                    o.Email
                FROM Items i
                LEFT JOIN Owners o ON i.OwnerId = o.Id
                WHERE 
                    (@search = '') OR
                    i.Name LIKE '%' + @search + '%' OR
                    ISNULL(o.FullName, '') LIKE '%' + @search + '%' OR
                    ISNULL(o.Phone, '') LIKE '%' + @search + '%'
            ", conn);

            cmd.Parameters.AddWithValue("@search", string.IsNullOrWhiteSpace(search) ? "" : search);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Items.Add(new Item
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString(),
                    OwnerId = reader["OwnerId"] == DBNull.Value ? null : (int?)reader["OwnerId"],
                    OwnerName = reader["FullName"] == DBNull.Value ? "—" : reader["FullName"].ToString(),
                    OwnerPhone = reader["Phone"] == DBNull.Value ? "—" : FormatPhone(reader["Phone"].ToString()),
                    OwnerEmail = reader["Email"] == DBNull.Value ? "—" : reader["Email"].ToString()
                });
            }
        }

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "—";

            if (phone.Length == 11)
                return $"+7 ({phone.Substring(1, 3)}) {phone.Substring(4, 3)}-{phone.Substring(7, 2)}-{phone.Substring(9, 2)}";

            return phone;
        }

        [RelayCommand]
        public void Add()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Items (Name, Description, Status) VALUES (@n,@d,'Найдено')",
                conn);

            cmd.Parameters.AddWithValue("@n", Name);
            cmd.Parameters.AddWithValue("@d", Description ?? "");

            cmd.ExecuteNonQuery();

            Name = "";
            Description = "";

            Load();
        }

        [RelayCommand]
        public void Delete()
        {
            if (SelectedItem == null)
                return;

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "DELETE FROM Items WHERE Id=@id",
                conn);

            cmd.Parameters.AddWithValue("@id", SelectedItem.Id);

            cmd.ExecuteNonQuery();

            Load();
        }

        [RelayCommand]
        public void Update()
        {
            if (SelectedItem == null)
                return;

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "UPDATE Items SET Name=@n, Description=@d WHERE Id=@id",
                conn);

            cmd.Parameters.AddWithValue("@n", SelectedItem.Name);
            cmd.Parameters.AddWithValue("@d", SelectedItem.Description ?? "");
            cmd.Parameters.AddWithValue("@id", SelectedItem.Id);

            cmd.ExecuteNonQuery();

            Load();
        }

        [RelayCommand]
        public void GiveItem()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Выберите вещь!");
                return;
            }

            LostAndFound3.Views.MainWindow.MainFrameStatic.Navigate(
                new LostAndFound3.Views.GiveItemPage(SelectedItem.Id));
        }

        [RelayCommand]
        public void Search()
        {
            Load(SearchText);
        }

        partial void OnSearchTextChanged(string value)
        {
            Load(value);
        }
    }
}
