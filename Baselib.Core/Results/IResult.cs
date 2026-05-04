namespace Baselib.Core.Results;

public interface IResult
{
    bool Success { get; }
    string Message { get; }
    int StatusCode { get; }
}

public interface IDataResult<T> : IResult
{
    T Data { get; }
}