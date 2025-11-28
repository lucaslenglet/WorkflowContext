using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext.Core;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWorkflowContext() =>
            services.AddScoped<IWorkflowContextBuilder, WorkflowContextBuilder>();   
    }
}
