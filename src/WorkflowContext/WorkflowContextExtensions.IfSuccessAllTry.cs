using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : ICombine<TError>, IFromException<TError>
    {
        /// <summary>
        /// Runs every one of <paramref name="steps"/>, only while the state is a success, without stopping at the first
        /// failure: the errors of all the failing steps are merged with <c>TError.Combine</c>, in order, and become the state.
        /// An exception thrown by a step becomes that step's error through <c>TError.From</c>, and the next steps still run.
        /// </summary>
        /// <param name="steps">The steps, run one after the other in this order.</param>
        public WorkflowContext<TData, TError> IfSuccessAllTry(
            params Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllTryInternal(steps));
        }

        /// <inheritdoc cref="IfSuccessAllTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAllTry(
            params Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllTryInternal(steps));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : ICombine<TError>, IFromException<TError>
    {
        /// <inheritdoc cref="IfSuccessAllTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAllTry(
            params Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllTryInternal(steps));
        }

        /// <inheritdoc cref="IfSuccessAllTry{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAllTry(
            params Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllTryInternal(steps));
        }
    }
}
