using System;

namespace WorkflowContext;

public static class WorkflowState
{
    public static SuccessWorkflowState Success() => new();
    public static WorkflowState<TError> Success<TError>() => WorkflowState<TError>.Success();
    public static WorkflowState<TError> Failure<TError>(TError error) => WorkflowState<TError>.Failure(error);
}

public readonly struct SuccessWorkflowState;

public readonly struct WorkflowState<TError>
{
    public WorkflowState(TError error)
    {
        Error = error;
        IsFailure = true;
    }

    public TError Error { get; }
    public bool IsFailure { get; } = false;
    public bool IsSuccess => !IsFailure;

    public static implicit operator WorkflowState<TError>(TError error) =>
        new(error);

    public WorkflowState<TError2> Map<TError2>(Func<TError, TError2> mapper) =>
        IsFailure ? mapper(Error) : new WorkflowState<TError2>();

    public void Match(Action onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    public static implicit operator WorkflowState<TError>(SuccessWorkflowState _) => new();

    public static WorkflowState<TError> Success() => new();

    public static WorkflowState<TError> Failure(TError error) => new(error);
}