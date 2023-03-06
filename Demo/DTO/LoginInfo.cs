using System;

namespace CAE.DTO
{
    /// <summary>
    /// 需要登录以后才能使用的请求必须继承这个
    /// </summary>
    public abstract class LoginedRequest : DTOBase
    {
        public string Token { get; set; }
    }
    public abstract class DTOBase
    {
        public abstract MessageType type { get; }

        public CommonResult ToResult()
        {
            var res = new CommonResult()
            {
                type = type,
                state = 1
            };
            return res;
        }
        public CommonResult ToResult(string message)
        {
            var res = new CommonResult()
            {
                type = type,
                state = 500,
                message = message
            };
            return res;
        }
    }
    public class LoginInfo : DTOBase
    {
        public string username { get; set; }
        public string password { get; set; }

        public override MessageType type => MessageType.登录;
    }
    public class LoginResult
    {
        public string token { get; set; }
        public bool isAdmin { get; set; }
    }
    public class AddUserRequest : LoginedRequest
    {
        public string username { get; set; }
        public string password { get; set; }

        public override MessageType type => MessageType.注册;
    }
    public class RemoveUserRequest : LoginedRequest
    {
        public Guid id { get; set; }

        public override MessageType type => MessageType.删除用户;
    }
    public class 修改密码 : LoginedRequest
    {
        //public string username { get; set; }
        //public string password { get; set; }
        public string newpassword { get; set; }

        public override MessageType type => MessageType.修改密码;
    }
    public class 任务开始 : LoginedRequest
    {
        public Guid TaskID { get; set; }
        public override MessageType type => MessageType.任务开始;
    }
    public class 任务暂停 : LoginedRequest
    {
        public Guid TaskID { get; set; }
        public override MessageType type => MessageType.任务暂停;
    }
    public class 任务停止 : LoginedRequest
    {
        public Guid TaskID { get; set; }
        public override MessageType type => MessageType.任务停止;
    }
    public class TaskDownRequest : LoginedRequest
    {
        public Guid TaskID { get; set; }
        public override MessageType type => MessageType.下载文件;
    }
}