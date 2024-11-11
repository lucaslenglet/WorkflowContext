using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowContext;
using WorkflowContext.ConsoleApp;

await new ServiceCollection()
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddTransient<Demo>()
    .AddScoped<ScopedService>()
    .AddWorkflowContext()
    .AddLogging(builder =>
        builder.ClearProviders().AddConsole())
    .BuildServiceProvider()
    .GetRequiredService<Demo>()
    .StartAsync();


// TODOS :
// - ExecuteMap()
// - Create child context from any context (scoped ?)
// - Add ability to inject middlewares inside context that wills automatically be executed at the right time between steps