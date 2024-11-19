using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlanner<TData, TError> planner)
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteScoped(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlanner<TData, TError> planner)
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteScoped(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteScoped(planner));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        return await context.IfSuccessDoInternal(ctx => ctx.ExecuteScoped(planner));
    }
}
