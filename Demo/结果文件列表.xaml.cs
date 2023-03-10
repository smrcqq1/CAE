using CAE.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CAE.Demo
{
    /// <summary>
    /// 结果文件列表.xaml 的交互逻辑
    /// </summary>
    public partial class 结果文件列表 : Window
    {
        public 结果文件列表(任务 task)
        {
            InitializeComponent();
            Loaded += 结果文件列表_Loaded;
            DataContext = Model = new ViewModels.结果文件VM(task);
        }
        ViewModels.结果文件VM Model;
        private async void 结果文件列表_Loaded(object sender, RoutedEventArgs e)
        {
            await Model.请求下载结果文件列表();
        }

        private async void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            var el = sender as Button; 
            if (el == null) { return; }
            var data = el.DataContext as ViewModels.FileInfoVM;
            if (data == null) { return; };
            await Model.下载指定的结果文件(data);
        }

        private void btn_ChangDir_Click(object sender, RoutedEventArgs e)
        {
            var win = new System.Windows.Forms.FolderBrowserDialog();
            if(win.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            Model.SaveDir = win.SelectedPath;
        }

        private void btn_OpenDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Model.SaveDir);
            }
            catch(Exception ex)
            {
                VM.Instance.Alert = $"打开文件夹[{Model.SaveDir}]失败:{ex.Message}";
            }
        }

        private void btn_DownAll_Click(object sender, RoutedEventArgs e)
        {
            var el = sender as Button;
            if (el == null) { return; }
            el.IsEnabled = false;
            if (Model.Files == null)
            {
                return;
            }
            var tasks = new List<Task>();
            foreach (var item in Model.Files)
            {
                if (item.State)
                {
                    tasks.Add(Model.下载指定的结果文件(item));
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}