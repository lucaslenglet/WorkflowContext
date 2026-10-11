using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        /// <summary>
        /// Runs <paramref name="step"/> only while the state is a success; its result becomes the state. Any exception it
        /// throws, <see cref="OperationCanceledException"/> included, becomes a failure through <c>TError.From</c>.
        /// </summary>
        public WorkflowContext<TData, TError> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.ExecuteTry(step));
        }

        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.ExecuteTry(step));
        }

        /// <summary>
        /// Runs <paramref name="step"/> only while the state is a success and <paramref name="when"/> holds; otherwise
        /// the step is skipped and the state is left as it is.
        /// </summary>
        /// <param name="when">The condition on the context for the step to run.</param>
        /// <param name="step">The step.</param>
        public WorkflowContext<TData, TError> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.State.IsSuccess && when(ctx), ctx => ctx.ExecuteTry(step));
        }

        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.State.IsSuccess && when(ctx), ctx => ctx.ExecuteTry(step));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : IFromException<TError>
    {
        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.ExecuteTry(step));
        }

        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.ExecuteTry(step));
        }

        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, bool> when,
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return context.RunIfInternal(ctx => ctx.State.IsSuccess && when(ctx), ctx => ctx.ExecuteTry(step));
        }

        /// <inheritdoc cref="IfSuccessTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, bool}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}})"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessTry(
            Func<WorkflowContext<TData, TError>, bool> when,
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return context.RunIfInternal(ctx => ctx.State.IsSuccess && when(ctx), ctx => ctx.ExecuteTry(step));
        }
    }
}
