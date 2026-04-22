namespace GivingChampion.Common.Results;

public sealed record ServiceError(string Code, string Description);

public class AuthServiceResult
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<ServiceError> Errors { get; init; } = [];

    public static AuthServiceResult Success() => new() { Succeeded = true };

    public static AuthServiceResult Failure(params ServiceError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static AuthServiceResult Failure(IEnumerable<ServiceError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}

public sealed class AuthServiceResult<T> : AuthServiceResult
{
    public T? Data { get; init; }

    public static AuthServiceResult<T> Success(T data) =>
        new() { Succeeded = true, Data = data };

    public new static AuthServiceResult<T> Failure(params ServiceError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public new static AuthServiceResult<T> Failure(IEnumerable<ServiceError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}