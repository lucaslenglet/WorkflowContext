using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WorkflowContext
{
    public static partial class BaseContext
    {
        public static WorkflowContext<TValue, TError> IfSuccessScoped<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, WorkflowContext<TValue, TError>> action)
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

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, WorkflowContext<TValue, TError>> action)
        {
            var awaited = await context;
            return IfSuccessScoped(awaited, action);
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task<WorkflowContext<TValue, TError>>> action)
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

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task<WorkflowContext<TValue, TError>>> action)
        {
            var awaited = await context;
            return await IfSuccessScoped(awaited, action);
        }

        public static WorkflowContext<TValue, TError> IfSuccessScoped<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Action<WorkflowContext<TValue, TError>> action)
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

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Action<WorkflowContext<TValue, TError>> action)
        {
            var ctx = await context;
            return IfSuccessScoped(ctx, action);
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task> action)
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

        public static async Task<WorkflowContext<TValue, TError>> IfSuccessScoped<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task> action)
        {
            var ctx = await context;
            return await IfSuccessScoped(ctx, action);
        }
    }
}
