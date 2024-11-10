using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> step)
    {
        return context.IfSuccessDoInternal(step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
    {
        return (await context).IfSuccess(step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        return await context.IfSuccessDoInternal(step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        return await (await context).IfSuccess(step);
    }

    public static WorkflowContext<TData, TError> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        return context.IfSuccessDoInternal(ctx => ctx.Result = step(ctx));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        return (await context).IfSuccess(step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        return await context.IfSuccessDoInternal(async ctx => ctx.Result = await step(ctx));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        return await (await context).IfSuccess(step);
    }

    public static WorkflowContext<TData, TError> IfSuccess<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFrom<TError2, TError>
    {
        return context.IfSuccessDoInternal(ctx => 
            context.Result = step(ctx).MapError(TError.From));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFrom<TError2, TError>
    {
        return (await context).IfSuccess(step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFrom<TError2, TError>
    {
        return await context.IfSuccessDoInternal(async ctx =>
            context.Result = (await step(ctx)).MapError(TError.From));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFrom<TError2, TError>
    {
        return await (await context).IfSuccess(step);
    }
}
