using System;

namespace CAE.DTO
{
    public class 刷新任务列表Request : LoginedRequest
    {
        public static 刷新任务列表Request Instance = new 刷新任务列表Request();
        public override MessageType type => MessageType.刷新任务列表;
    }
    public class 刷新任务列表 : CommonResult
    {
        public 刷新任务列表()
        {
            type = MessageType.刷新任务列表;
        }

        public 任务[] Tasks { get; set; }
    }

    public class 任务
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public DateTime? CreateTime { get; set; }

        public TaskState State { get; set; }
    }
}
