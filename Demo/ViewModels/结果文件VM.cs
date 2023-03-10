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
        public 结果文件VM(任务 task)
        {
            Task = task;
            SaveDir = Path.Combine(System.Environment.CurrentDirectory, "结果文件", task.ID.ToString());
        }
        private List<FileInfoVM> files;
        任务 Task;
        private string saveDir;

        public List<FileInfoVM> Files { get => files; set { files = value; OnPropertyChanged(); } }

        public string SaveDir { get => saveDir; set { saveDir = value;OnPropertyChanged(); } }
        #region 请求下载结果文件列表
        public async Task<bool> 请求下载结果文件列表()
        {
            VM.Instance.Alert = "发送任务结果文件列表请求";
            var res = await VM.Instance.Connection.Send(new 获取任务结果文件列表Request()
            {
                ID = Task.ID
            });
            if (!res) { return false; }
            var result = await VM.Instance.Connection.ReceiveMessage<CAE.DTO.FileInfo[]>();
            if (result == null || !result.Success || result.Data == null)
            {
                VM.Instance.Alert = "获取任务结果文件列表失败";
                return false;
            }
            if (!string.IsNullOrEmpty(result.message))
            {
                VM.Instance.Alert = result.message;
                return false;
            }
            VM.Instance.Alert = "获取任务结果文件列表成功";
            Files = result.Data?.Select(o => new FileInfoVM()
            {
                Name = o.FileName,
                Length = o.FileSize
            }).ToList();
            return true;
        }
        #endregion 请求下载结果文件列表

        #region 下载指定的结果文件
        const int SizePerDown = 1024;
        public async Task<bool> 下载指定的结果文件(FileInfoVM file)
        {
            if (file.State)
            {
                return false;
            }
            VM.Instance.Alert = "开始下载：" + file.Name;
            file.BtnContent = "下载中...";
            try
            {
                file.Process = 1;
                try
                {
                    if (!Directory.Exists(SaveDir))
                    {
                        Directory.CreateDirectory(SaveDir);
                    }
                }catch(Exception ex)
                {
                    VM.Instance.Alert = "选取的下载路径不正确，请重新选择！";
                    DownFail(file);
                    return false;
                }
                var path = Path.Combine(SaveDir, file.Name);
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
                                TaskID = Task.ID,
                                Name = file.Name,
                                Start = file.Process,
                                End = end
                            });
                            if (!res)
                            {
                                VM.Instance.Alert = "下载出错:" + file.Name;
                                file.Process = -1;
                                DownFail(file);
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
            }
            catch (Exception ex)
            {
                VM.Instance.Alert = "下载异常:" + ex.Message;
                file.Process = -1;
                DownFail(file);
                return false;
            }
            DownSucess(file);
            return true;
        }

        void DownFail(FileInfoVM file)
        {
            file.BtnContent = "下载失败，点击重试";
        }
        void DownSucess(FileInfoVM file)
        {
            file.BtnContent = "重新下载";
        }
        #endregion 下载指定的结果文件
    }
}