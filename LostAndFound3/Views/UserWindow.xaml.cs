using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class UserWindow : Window
    {
        public static Frame UserFrameStatic { get; private set; }

        public UserWindow()
        {
            InitializeComponent();
            UserFrameStatic = UserFrame;
            UserFrame.Navigate(new UserPage());
        }
    }
}
