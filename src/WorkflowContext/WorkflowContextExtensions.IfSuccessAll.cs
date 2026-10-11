using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContextExtensions
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : ICombine<TError>
    {
        /// <summary>
        /// Runs every one of <paramref name="steps"/>, only while the state is a success, without stopping at the first
        /// failure: the errors of all the failing steps are merged with <c>TError.Combine</c>, in order, and become the state.
        /// Each step sees the state as it was before this operator. Exceptions propagate, and the remaining steps do not run.
        /// </summary>
        /// <param name="steps">The steps, run one after the other in this order.</param>
        public WorkflowContext<TData, TError> IfSuccessAll(
            params Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllInternal(steps));
        }

        /// <inheritdoc cref="IfSuccessAll{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAll(
            params Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllInternal(steps));
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : ICombine<TError>
    {
        /// <inheritdoc cref="IfSuccessAll{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAll(
            params Func<WorkflowContext<TData, TError>, WorkflowState<TError>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllInternal(steps));
        }

        /// <inheritdoc cref="IfSuccessAll{TData, TError}(WorkflowContext{TData, TError}, Func{WorkflowContext{TData, TError}, WorkflowState{TError}}[])"/>
        public Task<WorkflowContext<TData, TError>> IfSuccessAll(
            params Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>>[] steps)
        {
            ArgumentNullException.ThrowIfNull(steps);
            return context.RunIfInternal(static ctx => ctx.State.IsSuccess, ctx => ctx.RunAllInternal(steps));
        }
    }
}
