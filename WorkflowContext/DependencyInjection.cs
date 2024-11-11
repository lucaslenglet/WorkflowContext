using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowContext(this IServiceCollection services) =>
        services.AddScoped<IWorkflowContextBuilder, WorkflowContextBuilder>();
}
