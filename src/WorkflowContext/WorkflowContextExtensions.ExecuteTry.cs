using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        /// <summary>
        /// Runs <paramref name="step"/> whatever the state, turning any exception it throws into a failure through
        /// <c>TError.From</c>, <see cref="OperationCanceledException"/> included. A failure is applied, but it never
        /// replaces an earlier failure: only <c>OnFailure</c> does.
        /// </summary>
        public WorkflowContext<TData, TError> ExecuteTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            WorkflowState<TError> state;

            try
            {
                state = step(context);
            }
            catch (Exception ex)
            {
                state = TError.From(ex);
            }

            context.Apply(state);
            return context;
        }

        /// <inheritdoc cref="ExecuteTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            WorkflowState<TError> state;

            try
            {
                state = await step(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                state = TError.From(ex);
            }

            context.Apply(state);
            return context;
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : IFromException<TError>
    {
        /// <inheritdoc cref="ExecuteTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return (await context.ConfigureAwait(false)).ExecuteTry(step);
        }

        /// <inheritdoc cref="ExecuteTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return await (await context.ConfigureAwait(false)).ExecuteTry(step).ConfigureAwait(false);
        }
    }
}
