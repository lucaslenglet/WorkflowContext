using System.Threading;

namespace WorkflowContext;

/// <summary>
/// Creates workflow contexts bound to the current service scope. Registered by <c>AddWorkflowContext</c>.
/// </summary>
public interface IWorkflowContextFactory
{
    /// <summary>
    /// Creates a successful context carrying <paramref name="data"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the data the steps work on.</typeparam>
    /// <typeparam name="TError">The type of the error a failing step returns.</typeparam>
    /// <param name="data">The data the steps work on.</param>
    /// <param name="cancellationToken">A token the steps can observe.</param>
    WorkflowContext<TData, TError> Create<TData, TError>(TData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a successful context carrying a new instance of <typeparamref name="TData"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the data the steps work on.</typeparam>
    /// <typeparam name="TError">The type of the error a failing step returns.</typeparam>
    /// <param name="cancellationToken">A token the steps can observe.</param>
    WorkflowContext<TData, TError> Create<TData, TError>(CancellationToken cancellationToken = default)
        where TData : new();
}
