using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace LostAndFound.Views
{
    public partial class GiveItemWindow : Window
    {
        private string connectionString =
            "Server=IVAN\\SQLEXPRESS;Database=LostAndFoundDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private int _itemId;

        public GiveItemWindow(int itemId)
        {
            InitializeComponent();
            _itemId = itemId;
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

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("SELECT Id, FullName, Phone, Email  FROM Owners", conn);
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
            }

            UsersGrid.ItemsSource = list;
        }

        private void Give_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is Owner owner)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(
    "UPDATE Items SET OwnerId=@oid, Status='Выдано' WHERE Id=@id",
    conn);

                    cmd.Parameters.AddWithValue("@oid", owner.Id);
                    cmd.Parameters.AddWithValue("@id", _itemId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Вещь выдана!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите пользователя!");
            }
        }
    }
}