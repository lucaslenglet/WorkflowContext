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