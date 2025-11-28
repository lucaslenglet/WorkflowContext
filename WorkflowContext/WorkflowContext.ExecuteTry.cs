using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
        where TError : IFromException<TError>
    {
        public WorkflowContext<TData, TError> ExecuteTry(
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

        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            try
            {
                context.State = await step(context);
            }
            catch (Exception ex)
            {
                context.State = TError.From(ex);
            }

            return context;
        }
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
        where TError : IFromException<TError>
    {
        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return (await context).ExecuteTry(step);
        }

        public async Task<WorkflowContext<TData, TError>> ExecuteTry(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return await (await context).ExecuteTry(step);
        }
    }
}