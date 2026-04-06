using LostAndFound.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void OpenRequests_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrameStatic.Navigate(new RequestsPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LostAndFound.Views.LoginWindow();
            loginWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
