using LostAndFound.ViewModels;
using System.Windows;

namespace LostAndFound3.Views
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

