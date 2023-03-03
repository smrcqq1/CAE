using System.Windows;

namespace CAE.Demo
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = VM.Instance;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            var res = await VM.Instance.Login();
            if (res)
            {
                Close();
                return;
            }
            btnLogin.IsEnabled = true;
        }
    }
}
