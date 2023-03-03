namespace CAE.Demo.ViewModels
{
    internal class FileInfoVM : NotifyModel
    {
        private int process;
        /// <summary>
        /// 下载进度
        /// </summary>
        public int Process { get => process; set { process = value;OnPropertyChanged();OnPropertyChanged(nameof(State)); } }
        /// <summary>
        /// 显示名称，用于列表显示使用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整相对路径，用于下载,不显示给用户
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 文件总长度，前端根据长度来决定是否分片下载
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 标识文件下载状态
        /// </summary>
        public int State
        {
            get {
                if (process > 0)
                {
                    if(process < Length)
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }
    }
}