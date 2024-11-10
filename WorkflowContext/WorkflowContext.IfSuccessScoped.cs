using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        return context.IfSuccessDoInternal(ctx =>
        {
            var baseScope = ctx.Services;

            using (var scope = ctx.Services.CreateScope())
            {
                ctx.Services = scope.ServiceProvider;

                ctx = action(ctx);
            }

            ctx.Services = baseScope;

            return ctx;
        });
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
    {
        return (await context).IfSuccessScoped(action);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        return await context.IfSuccessDoInternal(async ctx =>
        {
            var baseScope = ctx.Services;

            using (var scope = ctx.Services.CreateScope())
            {
                ctx.Services = scope.ServiceProvider;

                ctx = await action(ctx);
            }

            ctx.Services = baseScope;

            return ctx;
        });
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
    {
        return await (await context).IfSuccessScoped(action);
    }

    public static WorkflowContext<TData, TError> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Action<WorkflowContext<TData, TError>> action)
    {
        return context.IfSuccessDoInternal(ctx =>
        {
            var baseScope = ctx.Services;

            using (var scope = ctx.Services.CreateScope())
            {
                ctx.Services = scope.ServiceProvider;

                action(ctx);
            }

            ctx.Services = baseScope;

            return ctx;
        });
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Action<WorkflowContext<TData, TError>> action)
    {
        return (await context).IfSuccessScoped(action);
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this WorkflowContext<TData, TError> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        return await context.IfSuccessScoped(async ctx =>
        {
            var baseScope = ctx.Services;

            using (var scope = ctx.Services.CreateScope())
            {
                ctx.Services = scope.ServiceProvider;

                await action(ctx);
            }

            ctx.Services = baseScope;

            return ctx;
        });
    }

    public static async Task<WorkflowContext<TData, TError>> IfSuccessScoped<TData, TError>(
        this Task<WorkflowContext<TData, TError>> context, Func<WorkflowContext<TData, TError>, Task> action)
    {
        return await (await context).IfSuccessScoped(action);
    }
}
