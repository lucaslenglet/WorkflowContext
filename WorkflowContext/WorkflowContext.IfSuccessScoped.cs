using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> action)
    {
        return context.IfSuccessDoInternal(ctx => ctx.ExecuteScoped(action));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> action)
    {
        return await context.IfSuccessScoped(ctx => ctx.ExecuteScoped(action));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        return await context.IfSuccessScoped(ctx => ctx.ExecuteScoped(action));
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        return await context.IfSuccessScoped(ctx => ctx.ExecuteScoped(action));
    }
}
