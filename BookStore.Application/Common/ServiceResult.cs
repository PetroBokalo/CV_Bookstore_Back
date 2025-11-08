
using Microsoft.AspNetCore.Http;

namespace BookStore.Application.Common
{

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        public static ServiceResult Ok (string message = "", int statuscode = StatusCodes.Status204NoContent) =>
            new ServiceResult { Success = true, Message = message, StatusCode = statuscode };

        public static ServiceResult Fail(string message = "Bad request", int statuscode = StatusCodes.Status400BadRequest) =>
            new ServiceResult { Success = false, Message = message, StatusCode = statuscode };

    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }

        
        public static ServiceResult<T> Ok(T data, string message = "", int statuscode = StatusCodes.Status200OK) =>
            new ServiceResult<T> { Success = true, Message = message, Data = data, StatusCode = statuscode };


        public static ServiceResult<T> Fail(T data, string message = "Bad request", int statuscode = StatusCodes.Status400BadRequest) =>
            new ServiceResult<T> {Success = false, Data = data, Message = message, StatusCode = statuscode };

        public static ServiceResult<T> Fail(string message = "Bad request", int statuscode = StatusCodes.Status400BadRequest) =>
            new ServiceResult<T> { Success = false, Message = message, StatusCode = statuscode };

    }
}
