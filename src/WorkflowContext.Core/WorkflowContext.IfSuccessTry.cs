using System;
using System.Threading.Tasks;

namespace WorkflowContext.Core;

public static partial class WorkflowContext
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        public WorkflowContext<TData, TError> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
        }

        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : IFromException<TError>
    {
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
        }

        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
        }
    }
}