using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> step)
        where TError : IFromException<TError>
    {
        try
        {
            step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
        where TError : IFromException<TError>
    {
        return (await context).ExecuteTry(step);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        try
        {
            await step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
        where TError : IFromException<TError>
    {
        return await (await context).ExecuteTry(step);
    }

    public static WorkflowContext<TData, TError> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        try
        {
            context.Result = step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        return (await context).ExecuteTry(step);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        try
        {
            context.Result = await step(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return await ExecuteTry(awaited, step);
    }


    public static WorkflowContext<TData, TError> ExecuteTry<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        try
        {
            context.Result = step(context).MapError(TError.From);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError2>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return (await context).ExecuteTry(step);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        try
        {
            var result = await step(context);
            context.Result = result.MapError(TError.From);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError2>>> step)
        where TError : IFromException<TError>, IFrom<TError2, TError>
    {
        return await (await context).ExecuteTry(step);
    }
}
