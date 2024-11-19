using System;
using System.Threading.Tasks;

namespace WorkflowContext;

public static partial class WorkflowContext
{
    public static WorkflowContext<TData2, TError> ExecuteMap<TData, TError, TData2>(
        this WorkflowContext<TData, TError> context, Func<TData, TData2> mapper)
    {
        var data = mapper(context.Data);
        return new(context.Services, data);
    }

    public static async Task<WorkflowContext<TData2, TError>> ExecuteMap<TData, TError, TData2>(
        this Task<WorkflowContext<TData, TError>> context, Func<TData, TData2> mapper)
    {
        return (await context).ExecuteMap(mapper);
    }

    public static async Task<WorkflowContext<TData2, TError>> ExecuteMap<TData, TError, TData2>(
        this WorkflowContext<TData, TError> context, Func<TData, Task<TData2>> mapper)
    {
        var data = await mapper(context.Data);
        return new(context.Services, data);
    }

    public static async Task<WorkflowContext<TData2, TError>> ExecuteMap<TData, TError, TData2>(
        this Task<WorkflowContext<TData, TError>> context, Func<TData, Task<TData2>> mapper)
    {
        return await (await context).ExecuteMap(mapper);
    }
}