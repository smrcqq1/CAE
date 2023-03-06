using System;

namespace CAE.DTO
{
    public class NewTaskRequest : LoginedRequest
    {
        public Guid TaskID { get; set; }
        public override MessageType type => MessageType.创建任务;
        public string TaskName { get; set; } = "新任务";
        public UploadFile[] File { get; set; }
    }
    public class UploadFile
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
    }
}
