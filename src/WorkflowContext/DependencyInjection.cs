using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext;

/// <summary>
/// Registers WorkflowContext in a service collection.
/// </summary>
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers <see cref="IWorkflowContextFactory"/> as scoped, so contexts resolve services from the current scope.
        /// </summary>
        public IServiceCollection AddWorkflowContext() =>
            services.AddScoped<IWorkflowContextFactory, WorkflowContextFactory>();
    }
}
