using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LostAndFound.Models;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

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
        public void Load()
        {
            Items.Clear();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Items", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Items.Add(new Item
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString()
                });
            }
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

       
        partial void OnSearchTextChanged(string value)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string query;

            if (string.IsNullOrWhiteSpace(value))
            {
                query = "SELECT * FROM Items";
            }
            else
            {
                query = @"
            SELECT * FROM Items
            WHERE 
                Name LIKE @search OR
                Description LIKE @search";
            }

            var cmd = new SqlCommand(query, conn);

            if (!string.IsNullOrWhiteSpace(value))
                cmd.Parameters.AddWithValue("@search", "%" + value + "%");

            var reader = cmd.ExecuteReader();

            Items.Clear();

            while (reader.Read())
            {
                Items.Add(new Item
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString()
                });
            }
        }
    }
}