using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    internal static WorkflowContext<TData, TError> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        action(context);

        return context;
    }

    internal static WorkflowContext<TData, TError> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        return action(context);
    }

    internal static async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        await action(context);

        return context;
    }

    internal static async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        return await action(context);
    }
}
