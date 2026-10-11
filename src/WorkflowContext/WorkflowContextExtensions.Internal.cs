using System;
using System.Threading.Tasks;

namespace WorkflowContext;

/// <summary>
/// The operators chaining the steps of a workflow.
/// </summary>
/// <remarks>
/// Every operator accepts a synchronous or an asynchronous step, and is also available on a
/// <see cref="Task{TResult}"/> of a context, so synchronous and asynchronous steps mix freely in one pipeline.
/// Awaits use <c>ConfigureAwait(false)</c>: steps after an asynchronous one may run on a thread-pool thread.
/// </remarks>
public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        internal WorkflowContext<TData, TError> RunIfInternal(
            Func<WorkflowContext<TData, TError>, bool> condition,
            Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
        {
            return condition(context) ? action(context) : context;
        }

        internal Task<WorkflowContext<TData, TError>> RunIfInternal(
            Func<WorkflowContext<TData, TError>, bool> condition,
            Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
        {
            return condition(context) ? action(context) : Task.FromResult(context);
        }

        // Checks IsFailure first: reading Error on a success throws
        internal bool FailedWith(Func<TError, bool> when) =>
            context.State.IsFailure && when(context.State.Error);

        internal WorkflowContext<TData, TError> ReplaceState(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            context.State = step(context);
            return context;
        }

        internal async Task<WorkflowContext<TData, TError>> ReplaceState(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            context.State = await step(context).ConfigureAwait(false);
            return context;
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        internal async Task<WorkflowContext<TData, TError>> RunIfInternal(
            Func<WorkflowContext<TData, TError>, bool> condition,
            Func<WorkflowContext<TData, TError>, WorkflowContext<TData, TError>> action)
        {
            return (await context.ConfigureAwait(false)).RunIfInternal(condition, action);
        }

        internal async Task<WorkflowContext<TData, TError>> RunIfInternal(
            Func<WorkflowContext<TData, TError>, bool> condition,
            Func<WorkflowContext<TData, TError>, Task<WorkflowContext<TData, TError>>> action)
        {
            return await (await context.ConfigureAwait(false)).RunIfInternal(condition, action).ConfigureAwait(false);
        }
    }

    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        internal WorkflowContext<TData, TError> ReplaceStateTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            try
            {
                context.State = step(context);
            }
            catch (Exception ex)
            {
                context.State = TError.From(ex);
            }

            return context;
        }

        internal async Task<WorkflowContext<TData, TError>> ReplaceStateTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            try
            {
                context.State = await step(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.State = TError.From(ex);
            }

            return context;
        }
    }

    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : ICombine<TError>
    {
        internal WorkflowContext<TData, TError> RunAllInternal(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            WorkflowState<TError> state = default;

            foreach (var step in steps)
            {
                state = CombineInternal(state, step(context));
            }

            context.State = state;
            return context;
        }

        internal async Task<WorkflowContext<TData, TError>> RunAllInternal(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            WorkflowState<TError> state = default;

            foreach (var step in steps)
            {
                state = CombineInternal(state, await step(context).ConfigureAwait(false));
            }

            context.State = state;
            return context;
        }
    }

    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : ICombine<TError>, IFromException<TError>
    {
        internal WorkflowContext<TData, TError> RunAllTryInternal(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            WorkflowState<TError> state = default;

            foreach (var step in steps)
            {
                WorkflowState<TError> result;

                try
                {
                    result = step(context);
                }
                catch (Exception ex)
                {
                    result = TError.From(ex);
                }

                state = CombineInternal(state, result);
            }

            context.State = state;
            return context;
        }

        internal async Task<WorkflowContext<TData, TError>> RunAllTryInternal(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            WorkflowState<TError> state = default;

            foreach (var step in steps)
            {
                WorkflowState<TError> result;

                try
                {
                    result = await step(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    result = TError.From(ex);
                }

                state = CombineInternal(state, result);
            }

            context.State = state;
            return context;
        }
    }

    private static WorkflowState<TError> CombineInternal<TError>(WorkflowState<TError> current, WorkflowState<TError> next)
        where TError : ICombine<TError>
    {
        if (current.IsSuccess)
        {
            return next;
        }

        return next.IsFailure ? TError.Combine(current.Error, next.Error) : current;
    }
}
