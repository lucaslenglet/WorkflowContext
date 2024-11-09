using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            context = action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        var awaited = await context;
        return IfSuccessScoped(awaited, action);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            context = await action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        var awaited = await context;
        return await IfSuccessScoped(awaited, action);
    }

    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> action)
    {
        var ctx = await context;
        return IfSuccessScoped(ctx, action);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        if (context.Result.IsFailure)
        {
            return context;
        }

        var baseScope = context.Services;

        using (var scope = context.Services.CreateScope())
        {
            context.Services = scope.ServiceProvider;

            await action(context);
        }

        context.Services = baseScope;

        return context;
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        var ctx = await context;
        return await IfSuccessScoped(ctx, action);
    }
}
