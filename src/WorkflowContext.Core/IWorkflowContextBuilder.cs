using System;

namespace WorkflowContext.Core;

public interface IWorkflowContextBuilder
{
    IServiceProvider ServiceProvider { get; }

    WorkflowContextBuilder<TError> WithError<TError>();
}