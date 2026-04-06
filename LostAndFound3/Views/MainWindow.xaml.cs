using LostAndFound.ViewModels;
using System.Windows;

namespace LostAndFound.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}

