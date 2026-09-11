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

    public static IResult Ok(string message = "")
        => SuccessResult(message, 200);

    public static IResult NotFound(string message = "Kayıt bulunamadı")
        => ErrorResult(message, 404);

    public static IResult Unauthorized(string message = "Yetkisiz erişim")
        => ErrorResult(message, 401);

    public static IResult BadRequest(string message)
        => ErrorResult(message, 400);
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

    public static IDataResult<T> Ok(T data, string message = "")
        => SuccessDataResult(data, message, 200);

    public static IDataResult<T> Created(T data, string message = "")
        => SuccessDataResult(data, message, 201);

    public static IDataResult<T> NotFound(string message = "Kayıt bulunamadı")
        => ErrorDataResult(message, 404);

    public static IDataResult<T> Unauthorized(string message = "Yetkisiz erişim")
        => ErrorDataResult(message, 401);

    public static IDataResult<T> BadRequest(string message)
        => ErrorDataResult(message, 400);
}