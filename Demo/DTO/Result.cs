namespace CAE.DTO
{
    internal class Result<T> : CommonResult
    {
        public Result() { } 
        public Result(string message) : base(message) {
        }
        public Result(T data) :base("")
        {
            Data = data;
            state = 1;
        }
        public T Data { get; set; }
    }
}