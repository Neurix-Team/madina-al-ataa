// Result.cs (in GivingChampion.Common.DTO.Global or similar)
public class Result
{
    public bool Succeeded { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    public static Result Success() => new Result(true, null, null);
    public static Result Failure(string error, string? errorCode = null) => new Result(false, error, errorCode);

    public Result(bool succeeded, string? error, string? errorCode)
    {
        Succeeded = succeeded;
        Error = error;
        ErrorCode = errorCode;
    }
}

public class Result<T> : Result
{
    public T? Value { get; }

    public static Result<T> Success(T value) => new Result<T>(true, null, null, value);
    public static new Result<T> Failure(string error, string? errorCode = null) => new Result<T>(false, error, errorCode, default);

    private Result(bool succeeded, string? error, string? errorCode, T? value) : base(succeeded, error, errorCode)
    {
        Value = value;
    }
}