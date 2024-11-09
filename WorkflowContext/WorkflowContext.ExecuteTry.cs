using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class BaseContext
{
    public static WorkflowContext<TValue, TError> ExecuteTry<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Action<WorkflowContext<TValue, TError>> step)
        where TError : IFromException<TError>
    {
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

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Action<WorkflowContext<TValue, TError>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return ExecuteTry(awaited, step);
    }

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task> step)
        where TError : IFromException<TError>
    {
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

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return await ExecuteTry(awaited, step);
    }


    public static WorkflowContext<TValue, TError> ExecuteTry<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
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

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return ExecuteTry(awaited, step);
    }

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
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

    public static async Task<WorkflowContext<TValue, TError>> ExecuteTry<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
        where TError : IFromException<TError>
    {
        var awaited = await context;
        return await ExecuteTry(awaited, step);
    }
}
