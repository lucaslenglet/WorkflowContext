using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowContext;
using WorkflowContext.ConsoleApp;

await new ServiceCollection()
    // Setup application
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddTransient<Demo>()
    .AddScoped<ScopedService>()
    .AddWorkflowContext()
    .AddLogging(builder =>
        builder.ClearProviders().AddConsole())
    .BuildServiceProvider()
    // Start demo application
    .GetRequiredService<Demo>()
    .StartAsync();


// TODOS :
// - [X] ExecuteMap()
// - [/] Create child context from any context (scoped ?)
// - [ ] Add ability to inject middlewares inside context that wills automatically be executed at the right time between steps
// - [X] Replace UnitResult<> with some kind of WorkflowResult or WorkflowState
// - [X] Add some kind of workflow planner