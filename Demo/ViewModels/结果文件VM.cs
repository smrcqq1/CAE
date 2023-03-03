#region using
using CAE.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#endregion using
namespace CAE.Demo.ViewModels
{
    internal class 结果文件VM : NotifyModel
    {
        private List<FileInfoVM> files;

        public List<FileInfoVM> Files { get => files; set { files = value; OnPropertyChanged(); } }
        #region 请求下载结果文件列表
        public async Task<bool> 请求下载结果文件列表(Guid taskID)
        {
            VM.Instance.Alert = "发送任务结果文件列表请求";
            var res = await VM.Instance.Connection.Send(new 获取任务结果文件列表Request()
            {
                ID = taskID
            });
            if (!res) { return false; }
            var result = await VM.Instance.Connection.ReceiveMessage<任务结果文件列表>();
            if (result == null || !result.Success || result.Data == null)
            {
                VM.Instance.Alert = "获取任务结果文件列表失败";
                return false;
            }
            if (!string.IsNullOrEmpty(result.Message))
            {
                VM.Instance.Alert = result.Message;
                return false;
            }
            VM.Instance.Alert = "获取任务结果文件列表成功";
            Files = result.Data.Files?.Select(o => new FileInfoVM()
            {
                Name = o.Name,
                FullName = o.FullName,
                Length = o.Length
            }).ToList();
            return true;
        }
        #endregion 请求下载结果文件列表

        #region 下载指定的结果文件
        const int SizePerDown = 1024;
        public async Task<bool> 下载指定的结果文件(任务 task,FileInfoVM file)
        {
            VM.Instance.Alert = "开始下载：" + file.Name;
            file.Process = 0;
            var path = Path.Combine(System.Environment.CurrentDirectory,"结果文件",task.Name,file.Name);
            using (var conn = new Connection())
            {
                var len = file.Length;
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    fs.SetLength(len);
                    while (file.Process < len)
                    {
                        var end = file.Process + SizePerDown;
                        if (end > len)
                        {
                            end = len;
                        }
                        var res = await conn.Send(new DownLoadFileRequest()
                        {
                            FullName = file.FullName,
                            Start = file.Process,
                            End = end
                        });
                        if (!res)
                        {
                            VM.Instance.Alert = "下载出错:" + file.Name;
                            file.Process = -1;
                            return false;
                        }
                        var cnt = end - file.Process;
                        var data = await conn.ReceiveData(cnt);
                        await fs.WriteAsync(data, file.Process, cnt);
                        file.Process = end;
                    }
                }
            }
            file.Process = file.Length;
            return true;
        }
        #endregion 下载指定的结果文件
    }
}