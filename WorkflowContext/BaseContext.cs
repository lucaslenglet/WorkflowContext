using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WorkflowContext
{
    public abstract class BaseContext<TError>
    {
        protected BaseContext(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        public IServiceProvider Services { get; internal set; }

        public UnitResult<TError> Result { get; internal set; } = UnitResult.Success<TError>();
    }

    public static class BaseContext
    {
        public static TContext Then<TContext, TError>(this TContext context, Func<TContext, UnitResult<TError>> step)
            where TContext : BaseContext<TError>
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            context.Result = step(context);

            return context;
        }

        public static async Task<TContext> Then<TContext, TError>(this Task<TContext> context, Func<TContext, UnitResult<TError>> step)
            where TContext : BaseContext<TError>
        {
            var awaited = await context;
            return Then(awaited, step);
        }

        public static async Task<TContext> Then<TContext, TError>(this TContext context, Func<TContext, Task<UnitResult<TError>>> step)
            where TContext : BaseContext<TError>
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            context.Result = await step(context);

            return context;
        }

        public static async Task<TContext> Then<TContext, TError>(this Task<TContext> context, Func<TContext, Task<UnitResult<TError>>> step)
            where TContext : BaseContext<TError>
        {
            var awaited = await context;
            return await Then(awaited, step);
        }

        public static TContext Scoped<TContext, TError>(this TContext context, Action<TContext> action)
            where TContext : BaseContext<TError>
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

        public static async Task<TContext> Scoped<TContext, TError>(this Task<TContext> context, Action<TContext> action)
            where TContext : BaseContext<TError>
        {
            var ctx = await context;
            return Scoped<TContext, TError>(ctx, action);
        }

        public static async Task<TContext> Scoped<TContext, TError>(this TContext context, Func<TContext, Task> action)
            where TContext : BaseContext<TError>
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

        public static async Task<TContext> Scoped<TContext, TError>(this Task<TContext> context, Func<TContext, Task> action)
            where TContext : BaseContext<TError>
        {
            var ctx = await context;
            return await Scoped<TContext, TError>(ctx, action);
        }
    }
}