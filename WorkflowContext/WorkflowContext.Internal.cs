using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        internal WorkflowContext<TData, TError> IfSuccessDoInternal(
            Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
        {
            if (context.State.IsFailure)
            {
                return context;
            }

            return action(context);
        }

        internal async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal(
            Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
        {
            if (context.State.IsFailure)
            {
                return context;
            }

            return await action(context);
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        internal async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal(
            Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
        {
            return (await context).IfSuccessDoInternal(action);
        }

        internal async Task<WorkflowContext<TData, TError>> IfSuccessDoInternal(
            Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
        {
            return await (await context).IfSuccessDoInternal(action);
        }
    }
}