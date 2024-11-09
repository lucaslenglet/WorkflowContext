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
        if (context.Result.IsFailure)
        {
            return context;
        }

        try
        {
            step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.FromException(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return IfSuccessTry(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        try
        {
            await step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.FromException(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return await IfSuccessTry(awaited, step);
    }


    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        try
        {
            context.Result = step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.FromException(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return IfSuccessTry(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        try
        {
            context.Result = await step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.FromException(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return await IfSuccessTry(awaited, step);
    }
}
