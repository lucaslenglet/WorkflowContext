using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> step)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        step(context);

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
    {
        var awaited = await context;
        return IfSuccess(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        await step(context);

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        var awaited = await context;
        return await IfSuccess(awaited, step);
    }


    public static WorkflowContext<TData, TError> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        context.Result = step(context);

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        var awaited = await context;
        return IfSuccess(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        context.Result = await step(context);

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccess<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        var awaited = await context;
        return await IfSuccess(awaited, step);
    }
}
