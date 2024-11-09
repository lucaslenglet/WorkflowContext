using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> step)
    {
        step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
    {
        var awaited = await context;
        return Execute(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        var awaited = await context;
        return await Execute(awaited, step);
    }


    public static WorkflowContext<TData, TError> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        context.Result = step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, UnitResult<TError>> step)
    {
        var awaited = await context;
        return Execute(awaited, step);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        context.Result = await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<UnitResult<TError>>> step)
    {
        var awaited = await context;
        return await Execute(awaited, step);
    }
}
