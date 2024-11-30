using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
    {
        context.State = step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
    {
        return (await context).Execute(step);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
    {
        context.State = await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
    {
        return await (await context).Execute(step);
    }
}
