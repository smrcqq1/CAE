namespace CAE.Demo.ViewModels
{
    internal class FileInfoVM : NotifyModel
    {
        private int process;
        private string btnContent = "下载";
        private bool state;

        /// <summary>
        /// 下载进度
        /// </summary>
        public int Process
        {
            get => process;
            set
            {
                process = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProcessStr));
                if (process > 0 && process < Length)
                {
                    BtnContent = "下载中...";
                    state = false;
                    return;
                }
                BtnContent = "重新下载";
                state = true;
            }
        }
        /// <summary>
        /// 显示名称，用于列表显示使用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件总长度，前端根据长度来决定是否分片下载
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 标识文件下载状态
        /// </summary>
        public bool State
        {
            get => state; set { state = value;OnPropertyChanged(); }
        }
        public string ProcessStr
        {
            get
            {
                return $"{Process} / {Length}";
            }
        }
        public string BtnContent
        {
            get => btnContent; set { btnContent = value; OnPropertyChanged(); }
        }
    }
}