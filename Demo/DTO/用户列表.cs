using CAE.DTO;
using System;

namespace CAE.Demo.DTO
{
    internal class 用户列表Request : LoginedRequest
    {
        public override MessageType type => MessageType.获取用户列表;
    }
    public class 用户信息
    {
        public Guid id { get; set; }
        public string username { get; set; }
    }
}
