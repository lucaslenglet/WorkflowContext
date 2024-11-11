using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlanner<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlanner<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlannerAsync<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessTry<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlannerAsync<TData, TError> planner)
        where TError : IFromException<TError>
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteTry(planner));
    }
}