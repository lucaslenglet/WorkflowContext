using System;

namespace WorkflowContext;

public interface IWorkflowContextBuilder
{
    IServiceProvider ServiceProvider { get; }

    WorkflowContextBuilder<TError> WithError<TError>();
}