using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    internal static WorkflowContext<TData, TError> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        if (context.State.IsFailure)
        {
            return context;
        }

        return action(context);
    }

    internal static async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        return (await context).IfSuccessDoInternal(action);
    }

    internal static async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        if (context.State.IsFailure)
        {
            return context;
        }

        return await action(context);
    }

    internal static async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        return await (await context).IfSuccessDoInternal(action);
    }
}