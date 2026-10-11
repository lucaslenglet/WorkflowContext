using System;

namespace WorkflowContext;

/// <summary>
/// Factory methods for <see cref="WorkflowState{TError}"/>.
/// </summary>
public static class WorkflowState
{
    /// <summary>
    /// Returns a success that converts implicitly to a <see cref="WorkflowState{TError}"/> of any error type,
    /// so a step can return it without naming its error type.
    /// </summary>
    public static SuccessWorkflowState Success() => new();

    /// <summary>
    /// Returns a successful state.
    /// </summary>
    /// <typeparam name="TError">The type of the error the state would carry on failure.</typeparam>
    public static WorkflowState<TError> Success<TError>() => WorkflowState<TError>.Success();

    /// <summary>
    /// Returns a failed state carrying <paramref name="error"/>.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error. Must not be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static WorkflowState<TError> Failure<TError>(TError error) => WorkflowState<TError>.Failure(error);
}

/// <summary>
/// A success that is not bound to an error type yet. It converts implicitly to any <see cref="WorkflowState{TError}"/>.
/// </summary>
public readonly struct SuccessWorkflowState;

/// <summary>
/// The outcome of a workflow step: either a success, or a failure carrying an error.
/// </summary>
/// <remarks>
/// The default value of this struct is a success.
/// </remarks>
/// <typeparam name="TError">The type of the error carried on failure.</typeparam>
public readonly struct WorkflowState<TError>
{
    private readonly TError? _error;

    private WorkflowState(TError error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        _error = error;
        IsFailure = true;
    }

    /// <summary>
    /// The error of a failed state.
    /// </summary>
    /// <exception cref="InvalidOperationException">The state is a success.</exception>
    public TError Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("A successful state has no error.");

    /// <summary>
    /// Whether the state is a failure.
    /// </summary>
    public bool IsFailure { get; }

    /// <summary>
    /// Whether the state is a success.
    /// </summary>
    public bool IsSuccess => !IsFailure;

    /// <summary>
    /// Converts an error into a failed state.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static implicit operator WorkflowState<TError>(TError error) =>
        new(error);

    /// <summary>
    /// Converts an untyped success into a successful state.
    /// </summary>
    public static implicit operator WorkflowState<TError>(SuccessWorkflowState _) => default;

    /// <summary>
    /// Returns a successful state.
    /// </summary>
    public static WorkflowState<TError> Success() => default;

    /// <summary>
    /// Returns a failed state carrying <paramref name="error"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static WorkflowState<TError> Failure(TError error) => new(error);

    /// <summary>
    /// Converts the error of a failed state with <paramref name="mapper"/>. A success stays a success.
    /// </summary>
    /// <typeparam name="TError2">The type of the converted error.</typeparam>
    /// <param name="mapper">Converts the error. Called only on failure.</param>
    public WorkflowState<TError2> MapError<TError2>(Func<TError, TError2> mapper) =>
        IsFailure ? WorkflowState<TError2>.Failure(mapper(Error)) : default;

    /// <summary>
    /// Calls <paramref name="onSuccess"/> or <paramref name="onFailure"/>, depending on the state.
    /// </summary>
    public void Match(Action onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }

    /// <summary>
    /// Returns the result of <paramref name="onSuccess"/> or <paramref name="onFailure"/>, depending on the state.
    /// </summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<TError, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);
}
