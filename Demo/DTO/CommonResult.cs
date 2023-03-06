namespace CAE.DTO
{
    public class CommonResult
    {
        public CommonResult() { }
        public CommonResult(string message)
        {
            msg = message;
            state = 0;
        }
        public int state { get; set; }
        public string msg { get; set; }
        public MessageType type { get;set; }
        public bool Success
        {
            get
            {
                return state == 1;
            }
        }
    }
}
