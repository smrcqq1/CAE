#region using
using CAE.Demo.DTO;
using CAE.Demo.ViewModels;
using CAE.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Documents;
#endregion using
namespace CAE.Demo
{
    internal class VM : NotifyModel
    {
        #region 字段
        private Userinfo userinfo = new Userinfo();
        private bool logined = false;
        private string alert;
        private bool connected;
        private string message;
        private Connection connection = new Connection();
        private 任务[] tasks;
        private string cancha;
        #endregion 字段

        #region 属性
        public 任务[] Tasks { get => tasks; set { tasks = value; OnPropertyChanged(); } }
        public string Cancha { get => cancha; set { cancha = value; OnPropertyChanged(); } }

        public static VM Instance { get; } = new VM();

        public Userinfo Userinfo
        {
            get => userinfo; set { userinfo = value; OnPropertyChanged(); }
        }

        public bool Logined { get => logined; set { logined = value; OnPropertyChanged(); } }
        public bool Connected { get => connected; set { connected = value; OnPropertyChanged(); } }
        public string Alert { get => alert; set { alert += value; alert += "\r\n"; OnPropertyChanged(); } }
        public string Message { get => message; set { message = value; OnPropertyChanged(); } }

        public Connection Connection { get => connection; private set => connection = value; }
        #endregion 属性

        #region 登录
        public async Task<bool> Login()
        {
            if (string.IsNullOrEmpty(Userinfo.Username))
            {
                Message = "请输入用户名";
                return false;
            }
            if (string.IsNullOrEmpty(Userinfo.IP) || !IPAddress.TryParse(Userinfo.IP, out var addr))
            {
                Message = "请输入正确的服务器IP";
                return false;
            }
            if (!Userinfo.Port.HasValue || Userinfo.Port < 1 || Userinfo.Port > 60000)
            {
                Message = "请输入正确的服务器端口号";
                return false;
            }
            if (string.IsNullOrEmpty(Userinfo.Password))
            {
                Message = "请输入密码";
                return false;
            }
            Message = "登录中...";
            var res = await Connection.Send(new LoginInfo()
            {
                username = Userinfo.Username,
                password = Userinfo.Password
            });
            if (!res)
            {
                Message = "登录失败";
                return false;
            }
            var result = await Connection.ReceiveMessage<LoginResult>();
            if (result == null)
            {
                Message = "收取服务器回复失败";
                return false;
            }
            if (!result.Success)
            {
                Message = result.msg;
                return false;
            }
            if (result.Data != null && !string.IsNullOrEmpty(result.Data.token))
            {
                Userinfo.Token = result.Data.token;
                Userinfo.IsAdmin = result.Data.isAdmin;
                Logined = true;
                Message = "登录成功";
                return true;
            }
            Message = "登录失败：服务器返回数据错误";
            return false;
        }
        #endregion 登录

        #region 创建任务
        public async Task<bool> CreateTask(string[] files)
        {
            var fis = files.Select(o => {
                var f = new System.IO.FileInfo(o);
                return f;
            });
            var name = fis.Where(o => o.Extension.Equals(".pbd")).SingleOrDefault();
            if(name == null)
            {
                Alert= "请选择正确的待求解文件";
                return false;
            }
            var request = new NewTaskRequest()
            {
                File = fis.Select(f => new UploadFile() { FileName = f.Name, FileSize = (int)f.Length }).ToArray(),
                TaskName = name.Name.Remove(name.Name.IndexOf("."))
                    //TaskID = Guid.NewGuid()
                };
            var conn = new Connection();
            var res = await conn.Send(request);
            if (!res)
            {
                return false;
            }
            try
            {
                foreach (var item in files)
                {
                    if (!await 发送文件(conn,item))
                    {
                        Alert = "发送文件失败:" + item;
                        return false;
                    }
                }
                Alert = "创建任务成功";
                return await RefreshTasks();
            }
            catch (Exception ex)
            {
                Alert = "创建任务失败:" + ex.Message;
            }
            finally
            {
                conn.Dispose();
            }
            return false;
        }
        private List<用户信息> userList;

        async Task<bool> 发送文件(Connection conn,string file)
        {
            using (var fs = new FileStream(file, FileMode.Open))
            {
                int len = (int)fs.Length;
                var buffer = new byte[len];
                fs.Read(buffer, 0, len);
                return await conn.Send(buffer);
            }
        }
        #endregion 创建任务

        #region 刷新任务清单
        public async Task<bool> RefreshTasks()
        {
            var res = await Connection.Send(刷新任务列表Request.Instance);
            if (!res)
            {
                Alert = "发送刷新任务列表请求失败";
                return false;
            }
            Alert = "发送刷新任务列表请求成功";
            return await 刷新任务清单();
        }
        public async Task<bool> 刷新任务清单()
        {
            var result = await Connection.ReceiveMessage<任务[]>();
            if (result == null || !result.Success)
            {
                Alert = "刷新任务列表失败";
                return false;
            }
            Alert = "刷新任务列表成功";
            Tasks = result.Data;
            return true;
        }
        #endregion 刷新任务清单

        #region 任务控制
        public async Task<bool> 任务开始(Guid taskID)
        {
            return await ReceiveCommonResult(new 任务开始() { TaskID = taskID }, "开始任务");
        }
        public async Task<bool> 任务暂停(Guid taskID)
        {
            return await ReceiveCommonResult(new 任务暂停() { TaskID = taskID }, "暂停任务");
        }
        public async Task<bool> 任务停止(Guid taskID)
        {
            return await ReceiveCommonResult(new 任务停止() { TaskID = taskID }, "停止任务");
        }
        async Task<bool> ReceiveCommonResult(LoginedRequest request, string failMessage)
        {
            var res = await Connection.Send(request);
            if (!res)
            {
                Alert = "发送请求失败:" + failMessage;
                return false;
            }
            var result = await Connection.ReceiveMessage();
            if (result == null)
            {
                Alert = failMessage;
                return false;
            }
            if (!result.Success)
            {
                if (string.IsNullOrEmpty(result.msg))
                {
                    Alert = failMessage;
                    return false;
                }
                Alert = result.msg;
                return false;
            }
            return true;
        }
        #endregion 任务控制

        #region 修改密码
        public async Task<bool> ChangePassword()
        {
            if (string.IsNullOrEmpty(Userinfo.NewPassword))
            {
                Alert = "请输入新密码";
                return false;
            }
            var res = await Connection.Send(new 修改密码()
            {
                newpassword = Userinfo.NewPassword
            });
            if (!res) { Alert = "发送修改密码请求失败"; return false; }
            var result = await Connection.ReceiveMessage();
            if (result == null || !result.Success)
            {
                Alert = "修改密码失败" + result.msg; return false;
            }
            return true;
        }
        #endregion 修改密码

        #region 刷新用户列表
        public async Task<bool> RefreshUsers()
        {
            if (!Userinfo.IsAdmin)
            {
                return false;
            }
            await Connection.Send(new DTO.用户列表Request());
            var result = await Connection.ReceiveMessage<List<用户信息>>();
            if(result == null || !result.Success)
            {
                Alert = "获取用户列表失败" + result.msg; ;
                return false;
            }
            UserList = result.Data;
            return true;
        }
        public List<用户信息> UserList { get => userList; set { userList = value; OnPropertyChanged(); } }
        #endregion

        #region 新建用户
        public async Task<bool> AddUser()
        {
            if (!Userinfo.IsAdmin)
            {
                return false;
            }
            if (string.IsNullOrEmpty(Userinfo.Username))
            {
                Alert = "请输入新用户的用户名";
                return false;
            }
            if (string.IsNullOrEmpty(Userinfo.AddPassword))
            {
                Alert = "请输入新用户的密码";
                return false;
            }
            await Connection.Send(new AddUserRequest()
            {
                username =Userinfo.AddUserName,
                password =Userinfo.AddPassword,
            });
            var result = await Connection.ReceiveMessage();
            await RefreshUsers();
            if (result == null || !result.Success)
            {
                Alert = "新增用户失败:" + result.msg;
                return false;
            }
            return true;
        }
        #endregion 新建用户

        #region 删除用户
        public async Task<bool> RemoveUser(Guid id)
        {
            if (!Userinfo.IsAdmin)
            {
                return false;
            }
            await Connection.Send(new RemoveUserRequest()
            {
                id = id
            });
            var result = await Connection.ReceiveMessage();
            await RefreshUsers();
            if (result == null || !result.Success)
            {
                Alert = "删除用户失败:" + result.msg;
                return false;
            }
            return true;
        }
        #endregion

    }
}