using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWorkflowContext() =>
            services.AddScoped<IWorkflowContextBuilder, WorkflowContextBuilder>();   
    }
}
