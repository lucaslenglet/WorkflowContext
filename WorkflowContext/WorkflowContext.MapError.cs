using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData, TError2> MapError<TData, TError, TError2>(
        this WorkflowContext<TData, TError> context, Func<TError, TError2> mapper)
    {
        return new WorkflowContext<TData, TError2>(context.Services, context.Data)
        {
            Result = context.Result.Map(mapper)
        };
    }

    public static async Task<WorkflowContext<TData, TError2>> MapError<TData, TError, TError2>(
        this Task<WorkflowContext<TData, TError>> context, Func<TError, TError2> mapper)
    {
        return (await context).MapError(mapper);
    }
}
