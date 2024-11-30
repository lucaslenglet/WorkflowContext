using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }
}