namespace Baselib.Core.Results;

public class Result : IResult
{
    public bool Success { get; }
    public string Message { get; }
    public int StatusCode { get; }

    public Result(bool success, string message, int statusCode)
    {
        Success = success;
        Message = message;
        StatusCode = statusCode;
    }

    public static IResult SuccessResult(string message = "", int statusCode = 200)
        => new Result(true, message, statusCode);

    public static IResult ErrorResult(string message, int statusCode = 400)
        => new Result(false, message, statusCode);
}

public class DataResult<T> : IDataResult<T>
{
    public bool Success { get; }
    public string Message { get; }
    public int StatusCode { get; }
    public T Data { get; }

    public DataResult(bool success, T data, string message, int statusCode)
    {
        Success = success;
        Data = data;
        Message = message;
        StatusCode = statusCode;
    }

    public static IDataResult<T> SuccessDataResult(T data, string message = "", int statusCode = 200)
        => new DataResult<T>(true, data, message, statusCode);

    public static IDataResult<T> ErrorDataResult(string message, int statusCode = 400)
        => new DataResult<T>(false, default!, message, statusCode);
}