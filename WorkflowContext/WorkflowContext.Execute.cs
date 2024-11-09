using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class BaseContext
{
    public static WorkflowContext<TValue, TError> Execute<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Action<WorkflowContext<TValue, TError>> step)
    {
        step(context);
        return context;
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Action<WorkflowContext<TValue, TError>> step)
    {
        var awaited = await context;
        return Execute(awaited, step);
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task> step)
    {
        await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task> step)
    {
        var awaited = await context;
        return await Execute(awaited, step);
    }


    public static WorkflowContext<TValue, TError> Execute<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
    {
        context.Result = step(context);
        return context;
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
    {
        var awaited = await context;
        return Execute(awaited, step);
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
    {
        context.Result = await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TValue, TError>> Execute<TValue, TError>(
        this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
    {
        var awaited = await context;
        return await Execute(awaited, step);
    }
}
