namespace GivingChampion.Common.Results;

public sealed record ServiceError(string Code, string Description);

public class ServiceResult
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<ServiceError> Errors { get; init; } = [];

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult Failure(params ServiceError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static ServiceResult Failure(IEnumerable<ServiceError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}

public sealed class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data) =>
        new() { Succeeded = true, Data = data };

    public new static ServiceResult<T> Failure(params ServiceError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public new static ServiceResult<T> Failure(IEnumerable<ServiceError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}