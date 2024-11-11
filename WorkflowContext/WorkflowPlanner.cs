using System.Threading.Tasks;

namespace WorkflowContext;


public delegate WorkflowContext<TData, TError> WorkflowPlanner<TData, TError>(WorkflowContext<TData, TError> context);

public delegate Task<WorkflowContext<TData, TError>> WorkflowPlannerAsync<TData, TError>(WorkflowContext<TData, TError> context);

public interface IWorkflowContext<TData, TError>
{
    public static abstract WorkflowPlanner<TData, TError> Plan { get; }
}

public interface IWorkflowContextAsync<TData, TError>
{
    public static abstract WorkflowPlannerAsync<TData, TError> Plan { get; }
}