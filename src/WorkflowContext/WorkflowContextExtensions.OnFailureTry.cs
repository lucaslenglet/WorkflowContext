using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        /// <summary>
        /// Runs <paramref name="step"/> only when the state is a failure, and replaces the state with its result: return
        /// a success to recover, or a failure to replace the error. If the step throws, the error built by
        /// <c>TError.From</c> from the exception replaces the original one.
        /// </summary>
        public WorkflowContext<TData, TError> OnFailureTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceStateTry(step));
        }

        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceStateTry(step));
        }

        /// <summary>
        /// Runs <paramref name="step"/> only when the state is a failure whose error matches <paramref name="when"/>, and
        /// replaces the state with its result. If the step throws, the error built by <c>TError.From</c> from the
        /// exception replaces the original one.
        /// </summary>
        /// <param name="when">Selects the errors to handle. Other failures are kept as they are.</param>
        /// <param name="step">The recovery step.</param>
        public WorkflowContext<TData, TError> OnFailureTry(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceStateTry(step));
        }

        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceStateTry(step));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : IFromException<TError>
    {
        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceStateTry(step));
        }

        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsFailure, ctx => ctx.ReplaceStateTry(step));
        }

        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceStateTry(step));
        }

        /// <inheritdoc cref="OnFailureTry{TData, TError}(WorkflowContext{TData, TError}, Func{TError, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> OnFailureTry(
            Func<TError, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.FailedWith(when), ctx => ctx.ReplaceStateTry(step));
        }
    }
}
