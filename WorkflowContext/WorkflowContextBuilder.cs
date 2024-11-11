using System;

namespace WorkflowContext;

public class WorkflowContextBuilder(IServiceProvider serviceProvider) : IWorkflowContextBuilder
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public static IWorkflowContextBuilder Create(IServiceProvider services) =>
        new WorkflowContextBuilder(services);

    public WorkflowContextBuilder<TError> WithError<TError>() =>
        new(ServiceProvider);
}

public class WorkflowContextBuilder<TError>(IServiceProvider serviceProvider)
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public WorkflowContextBuilder<TData, TError> WithData<TData>(TData data) =>
        new(ServiceProvider, data);

    public WorkflowContextBuilder<TData, TError> WithData<TData>() where TData : new() =>
        new(ServiceProvider, new());
}

public class WorkflowContextBuilder<TData, TError>(IServiceProvider serviceProvider, TData data)
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public TData Data { get; } = data;

    public WorkflowContext<TData, TError> Build() =>
        new(ServiceProvider, Data);
}