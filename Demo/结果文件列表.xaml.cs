using CAE.DTO;
using System.Windows;
using System.Windows.Controls;

namespace CAE.Demo
{
    /// <summary>
    /// 结果文件列表.xaml 的交互逻辑
    /// </summary>
    public partial class 结果文件列表 : Window
    {
        public 结果文件列表(任务 task)
        {
            Task = task;
            InitializeComponent();
            Loaded += 结果文件列表_Loaded;
            DataContext = Model = new ViewModels.结果文件VM();
        }
        任务 Task;
        ViewModels.结果文件VM Model;
        private async void 结果文件列表_Loaded(object sender, RoutedEventArgs e)
        {
            await Model.请求下载结果文件列表(Task.ID);
        }

        private async void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            var el = sender as Button; 
            if (el == null) { return; }
            el.IsEnabled = false;
            el.Content = "下载中...";
            var data = el.DataContext as ViewModels.FileInfoVM;
            if (data == null) { return; };
            var res = await Model.下载指定的结果文件(Task,data);
            if (res)
            {
                el.Content = "重新下载";
            }
            else
            {
                el.Content = "下载失败，点击重试";
            }
        }
    }
}