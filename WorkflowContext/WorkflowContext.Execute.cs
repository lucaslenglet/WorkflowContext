using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    extension<TData, TError>(WorkflowContext<TData, TError> context)
    {
        public WorkflowContext<TData, TError> Execute(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            context.State = step(context);
            return context;
        }

        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            context.State = await step(context);
            return context;
        } 
    }

    extension<TData, TError>(Task<WorkflowContext<TData, TError>> context)
    {
        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, WorkflowState<TError>> step)
        {
            return (await context).Execute(step);
        }

        public async Task<WorkflowContext<TData, TError>> Execute(
            Func<WorkflowContext<TData, TError>, Task<WorkflowState<TError>>> step)
        {
            return await (await context).Execute(step);
        }  
    }
}
