namespace CAE.DTO
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        上传文件 = 0,
        排队 = 1,
        求解中 = 2,
        暂停 = 3,
        //停止 = 4,
        已完成 = 5
    }
}