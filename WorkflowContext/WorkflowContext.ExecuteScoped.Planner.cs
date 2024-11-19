using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> ExecuteScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlanner<TData, TError> planner)
    {
        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            context = planner(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlanner<TData, TError> planner)
    {
        return (await context).ExecuteScoped(planner);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            context = await planner(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, WorkflowPlannerAsync<TData, TError> planner)
    {
        return await (await context).ExecuteScoped(planner);
    }
}