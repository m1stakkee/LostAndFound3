using System.Windows;
using System.Windows.Controls;

namespace LostAndFound3.Views
{
    public partial class LoginWindow : Window
    {
        public static Frame LoginFrameStatic { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            LoginFrameStatic = LoginFrame;
            LoginFrame.Navigate(new LoginPage());
        }
    }
}
