using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
    {
        return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
    {
        return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
    {
        return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
    {
        return context.IfSuccessDoInternal(ctx => ctx.Execute(step));
    }
}