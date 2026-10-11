using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        /// <summary>
        /// Runs <paramref name="step"/> only when the state is a failure, and replaces the state with its result: return
        /// a success to recover, or a failure to replace the error. The step reads the current error from
        /// <c>context.State.Error</c>. Exceptions propagate.
        /// </summary>
        public WorkflowContext<TData, TError> OnFailure(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceState(step));
        }

        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceState(step));
        }

        /// <summary>
        /// Runs <paramref name="step"/> only when the state is a failure whose error matches <paramref name="when"/>, and
        /// replaces the state with its result: return a success to recover, or a failure to replace the error.
        /// Exceptions propagate.
        /// </summary>
        /// <param name="when">Selects the errors to handle. Other failures are kept as they are.</param>
        /// <param name="step">The recovery step.</param>
        public WorkflowContext<TData, TError> OnFailure(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceState(step));
        }

        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceState(step));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceState(step));
        }

        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceState(step));
        }

        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceState(step));
        }

        /// <inheritdoc cref="OnFailure{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailure(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceState(step));
        }
    }
}
