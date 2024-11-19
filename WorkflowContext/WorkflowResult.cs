using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static class WorkflowResult
{
    public static WorkflowResult<TError> Success<TError>() => new();
    public static WorkflowResult<TError> Failure<TError>(TError error) => new(error);
}

public readonly struct WorkflowResult<TError>
{
    public WorkflowResult(TError error)
    {
        Error = error;
        IsFailure = true;
    }

    public TError Error { get; }
    public bool IsFailure { get; } = false;
    public bool IsSuccess => !IsFailure;

    public static implicit operator WorkflowResult<TError>(TError error) =>
        new(error);

    public WorkflowResult<TError2> Map<TError2>(Func<TError, TError2> mapper) =>
        IsFailure ? mapper(Error) : new WorkflowResult<TError2>();

    public void Match(Action onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess)
            onSuccess();
        else
            onFailure(Error);
    }

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    public Task Match(Func<Task> onSuccess, Func<TError, Task> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    public Task<TResult> Match<TResult>(Func<Task<TResult>> onSuccess, Func<TError, Task<TResult>> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);
}