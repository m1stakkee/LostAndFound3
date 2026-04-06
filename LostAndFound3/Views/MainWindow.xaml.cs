using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class MainWindow : Window
    {
        public static Frame MainFrameStatic { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MainFrameStatic = MainFrame;
            MainFrame.Navigate(new MainPage());
        }
    }
}
