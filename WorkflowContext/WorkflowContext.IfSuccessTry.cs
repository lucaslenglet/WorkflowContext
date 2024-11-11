using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }


    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }

    public static Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(step));
    }
}