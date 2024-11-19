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
        return (await context).Execute(step);
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
        return await (await context).Execute(step);
    }


    public static WorkflowContext<TData, TError> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowResult<TError>> step)
    {
        context.Result = step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowResult<TError>> step)
    {
        return (await context).Execute(step);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowResult<TError>>> step)
    {
        context.Result = await step(context);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowResult<TError>>> step)
    {
        return await (await context).Execute(step);
    }

    public static WorkflowContext<TData, TError> Execute<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowResult<TError2>> step)
        where TError : IFrom<TError2, TError>
    {
        context.Result = step(context).Map(TError.From);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowResult<TError2>> step)
        where TError : IFrom<TError2, TError>
    {
        return (await context).Execute(step);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowResult<TError2>>> step)
        where TError : IFrom<TError2, TError>
    {
        var result = await step(context);
        context.Result = result.Map(TError.From);
        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowResult<TError2>>> step)
        where TError : IFrom<TError2, TError>
    {
        return await (await context).Execute(step);
    }
}
