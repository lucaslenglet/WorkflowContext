using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlanner<TData, TError> planner)
        where TError : IFromException<TError>
    {
        try
        {
            return planner(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlanner<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return (await context).ExecuteTry(planner);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlannerAsync<TData, TError> planner)
        where TError : IFromException<TError>
    {
        try
        {
            return await planner(context);
        }
        catch (Exception ex)
        {
            context.Result = TError.From(ex);
        }

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlannerAsync<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return await (await context).ExecuteTry(planner);
    }
}
