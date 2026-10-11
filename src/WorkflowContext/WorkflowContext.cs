using System;
using System.Threading;

namespace WorkflowContext;

/// <summary>
/// Carries the data, the state and the services of a workflow through its steps.
/// </summary>
/// <remarks>
/// The operators (<c>Execute</c>, <c>IfSuccess</c>, ...) update this instance in place and return it,
/// so the context is mutable and not thread-safe.
/// </remarks>
/// <typeparam name="TData">The type of the data the steps work on.</typeparam>
/// <typeparam name="TError">The type of the error a failing step returns.</typeparam>
/// <param name="serviceProvider">The services the steps can resolve.</param>
/// <param name="data">The data the steps work on.</param>
/// <param name="cancellationToken">A token the steps can observe. The operators never check it themselves.</param>
public sealed class WorkflowContext<TData, TError>(
    IServiceProvider serviceProvider,
    TData data,
    CancellationToken cancellationToken = default)
{
    /// <summary>
    /// The services the steps can resolve.
    /// </summary>
    public IServiceProvider Services { get; } = serviceProvider;

    /// <summary>
    /// The current state. It starts as a success.
    /// </summary>
    public WorkflowState<TError> State { get; internal set; }

    /// <summary>
    /// The data the steps work on.
    /// </summary>
    public TData Data { get; } = data;

    /// <summary>
    /// A token the steps can observe. The operators never check it themselves: handling cancellation is up to the steps.
    /// </summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;

    /// <summary>
    /// Deconstructs the context into its data and state.
    /// </summary>
    public void Deconstruct(out TData data, out WorkflowState<TError> state)
    {
        data = Data;
        state = State;
    }

    /// <summary>
    /// Deconstructs the context into its data, state and services.
    /// </summary>
    public void Deconstruct(out TData data, out WorkflowState<TError> state, out IServiceProvider serviceProvider)
    {
        Deconstruct(out data, out state);
        serviceProvider = Services;
    }

    /// <summary>
    /// Applies the state returned by a step, unless the context already failed: only <c>OnFailure</c> replaces a failure.
    /// </summary>
    internal void Apply(WorkflowState<TError> state)
    {
        if (State.IsSuccess)
        {
            State = state;
        }
    }
}
