using System;
using System.Threading;

namespace WorkflowContext;

internal sealed class WorkflowContextFactory(IServiceProvider serviceProvider) : IWorkflowContextFactory
{
    public WorkflowContext<TData, TError> Create<TData, TError>(TData data, CancellationToken cancellationToken = default) =>
        new(serviceProvider, data, cancellationToken);

    public WorkflowContext<TData, TError> Create<TData, TError>(CancellationToken cancellationToken = default)
        where TData : new() =>
        new(serviceProvider, new TData(), cancellationToken);
}
