namespace BookStoreAPI.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public int StatusCode { get; set; } 

        public static ServiceResult<T> Ok(T data, string message = "", int statuscode = 200) =>
            new ServiceResult<T> { Success = true, Message = message, Data = data, StatusCode = statuscode };

        public static ServiceResult<T> Fail(string message = "", int statuscode = 400) =>
            new ServiceResult<T> { Success = false, Message = message, StatusCode = statuscode };

        public static ServiceResult<T> Fail(T data, string message = "", int statuscode = 400) =>
            new ServiceResult<T> {Success = false, Data = data, Message = message, StatusCode = statuscode };

    }
}
