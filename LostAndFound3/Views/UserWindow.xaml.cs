using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LostAndFound.Views;
using LostAndFound.ViewModels;

namespace LostAndFound
{
    public partial class UserWindow : Window
    {
        private MainViewModel vm;

        public UserWindow()
        {
            InitializeComponent();

            vm = new MainViewModel();
            DataContext = vm; 
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
    }
}