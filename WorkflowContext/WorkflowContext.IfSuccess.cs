using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace WorkflowContext
{
    public static partial class BaseContext
    {
        public static WorkflowContext<TValue, TError> IfSuccess<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Action<WorkflowContext<TValue, TError>> step)
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            step(context);

            return context;
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Action<WorkflowContext<TValue, TError>> step)
        {
            var awaited = await context;
            return IfSuccess<TValue, TError>(awaited, step);
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task> step)
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            await step(context);

            return context;
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task> step)
        {
            var awaited = await context;
            return await IfSuccess<TValue, TError>(awaited, step);
        }


        public static WorkflowContext<TValue, TError> IfSuccess<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            context.Result = step(context);

            return context;
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, UnitResult<TError>> step)
        {
            var awaited = await context;
            return IfSuccess(awaited, step);
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this WorkflowContext<TValue, TError> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
        {
            if (context.Result.IsFailure)
            {
                return context;
            }

            context.Result = await step(context);

            return context;
        }

        public static async Task<WorkflowContext<TValue, TError>> IfSuccess<TValue, TError>(
            this Task<WorkflowContext<TValue, TError>> context, Func<WorkflowContext<TValue, TError>, Task<UnitResult<TError>>> step)
        {
            var awaited = await context;
            return await IfSuccess(awaited, step);
        }
    }
}
