namespace WorkflowContext.ConsoleApp;

internal class ScopedService
{
    public Guid ScopeId { get; } = Guid.NewGuid();
}
