using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowContext;
using WorkflowContext.ConsoleApp;

await new ServiceCollection()

    // Setup application
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddTransient<Demo>()
    .AddWorkflowContext()
    .AddLogging(builder =>
        builder.ClearProviders().AddConsole())

    .BuildServiceProvider()
    .GetRequiredService<Demo>()

    // Start demo application
    .StartAsync();