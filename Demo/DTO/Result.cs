namespace CAE.DTO
{
    internal class Result<T>
    {
        public Result(string message) {
            Message = message;
        }
        public Result(T data)
        {
            Data = data;
            Success = true;
        }
        public bool Success { get; set; } = false;
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
