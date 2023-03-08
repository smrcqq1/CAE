using System;

namespace CAE.DTO
{
    public class 刷新任务列表Request : LoginedRequest
    {
        public static 刷新任务列表Request Instance = new 刷新任务列表Request();
        public override MessageType type => MessageType.刷新任务列表;
    }

    public class 任务
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public string CreateTime { get; set; }

        public TaskState State { get; set; }
    }
}
