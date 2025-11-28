using System;
using System.Threading.Tasks;

namespace WorkflowContext.Core;

public static partial class WorkflowContext
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        public WorkflowContext<TData, TError> IfSuccess(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
        }

        public Task<WorkflowContext<TData, TError>> IfSuccess(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        public Task<WorkflowContext<TData, TError>> IfSuccess(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
        }

        public Task<WorkflowContext<TData, TError>> IfSuccess(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
        }
    }
}