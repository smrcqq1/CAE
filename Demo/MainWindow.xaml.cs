#region using
using CAE.Demo.DTO;
using CAE.DTO;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
#endregion using
namespace CAE.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var win = new LoginWindow();
            win.ShowDialog();
            if (!VM.Instance.Logined)
            {
                App.Current.Shutdown();
                return;
            }
            InitializeComponent();
            DataContext = VM.Instance;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await VM.Instance.刷新任务清单();
        }

        private async void NewTask_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != true)
            {
                return;
            }
            if (ofd.FileNames.Length != 2)
            {
                MessageBox.Show("请一次选择两个文件，分别为：input.txt和case.pbd");
            }
            await VM.Instance.CreateTask(ofd.FileNames.ToArray());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VM.Instance.Connection.Dispose();
        }

        任务 获取选中任务(object sender)
        {
            var item = sender as FrameworkElement;
            if (item == null)
            {
                return null;
            }
            return item.DataContext as 任务;
        }
        private async void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            var task = 获取选中任务(sender);
            if (task == null)
            {
                return;
            }
            await VM.Instance.任务暂停(task.ID);
        }

        private async void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            var task = 获取选中任务(sender);
            if (task == null)
            {
                return;
            }
            await VM.Instance.任务停止(task.ID);
        }
        private void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            var task = 获取选中任务(sender);
            if (task == null)
            {
                return;
            }
            var win = new 结果文件列表(task);
            win.Owner = this;
            win.ShowDialog();
        }

        private async void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            var task = 获取选中任务(sender);
            if (task == null)
            {
                return;
            }
            await VM.Instance.任务开始(task.ID);
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            tbx.ScrollToEnd();
        }

        private async void RefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            await VM.Instance.RefreshTasks();
        }

        private async void btn_Acc_Stop_Click(object sender, RoutedEventArgs e)
        {
            var el = sender as Button;
            if (el == null)
            {
                return;
            }
            el.IsEnabled = false;
            var data = el.DataContext as 用户信息;
            if(data == null)
            {
                return;
            }
            await VM.Instance.RemoveUser(data.id);
        }

        private void btn_Cancha_Click(object sender, RoutedEventArgs e)
        {
            var task = 获取选中任务(sender);
            if (task == null)
            {
                return;
            }
            var win = new Cancha(task.ID);
            win.ShowDialog();
        }

        private async void btn_ChangePwd_Click(object sender, RoutedEventArgs e)
        {
            await VM.Instance.ChangePassword();
        }

        private async void tab_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(tab.SelectedIndex == 0)
            {
                return;
            }
            await VM.Instance.RefreshUsers();
        }

        private async void btn_AddUser_Click(object sender, RoutedEventArgs e)
        {
            await VM.Instance.AddUser();
        }
    }
}
