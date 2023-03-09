using System;

namespace CAE.DTO
{
    public class 回传残差数据Request : LoginedRequest
    {
        public override MessageType type => MessageType.回传残差数据;

        public Guid TaskID { get; set; }
    }
    public class 回传残差数据
    {
        public Guid TaskID { get; set; }

        public string Result { get; set; }
        public bool IsEnd { get; set; } = false;
    }
}
