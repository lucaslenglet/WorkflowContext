using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlanner<TData, TError> planner)
    {
        return planner(context);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlanner<TData, TError> planner)
    {
        return (await context).Execute(planner);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        return await planner(context);
    }

    public static async Task<WorkflowContext<TData, TError>> Execute<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        return await (await context).Execute(planner);
    }
}
