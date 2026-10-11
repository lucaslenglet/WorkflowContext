using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        /// <summary>
        /// Runs <paramref name="step"/> whatever the state. A failure it returns is applied, but it never replaces
        /// an earlier failure: only <c>OnFailure</c> does. Exceptions propagate.
        /// </summary>
        public WorkflowContext<TData, TError> Execute(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            context.Apply(step(context));
            return context;
        }

        /// <inheritdoc cref="Execute{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            context.Apply(await step(context).ConfigureAwait(false));
            return context;
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        /// <inheritdoc cref="Execute{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return (await context.ConfigureAwait(false)).Execute(step);
        }

        /// <inheritdoc cref="Execute{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return await (await context.ConfigureAwait(false)).Execute(step).ConfigureAwait(false);
        }
    }
}
