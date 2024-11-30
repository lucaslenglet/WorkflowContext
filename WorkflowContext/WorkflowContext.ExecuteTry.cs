using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        where TError : IFromException<TError>
    {
        try
        {
            context.State = step(context);
        }
        catch (Exception ex)
        {
            context.State = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        where TError : IFromException<TError>
    {
        return (await context).ExecuteTry(step);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        where TError : IFromException<TError>
    {
        try
        {
            context.State = await step(context);
        }
        catch (Exception ex)
        {
            context.State = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        where TError : IFromException<TError>
    {
        return await (await context).ExecuteTry(step);
    }
}