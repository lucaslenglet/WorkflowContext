using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> ExecuteScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> action)
    {
        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> step)
    {
        return (await context).ExecuteScoped(step);
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            await action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> ExecuteScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> step)
    {
        return await (await context).ExecuteScoped(step);
    }
}