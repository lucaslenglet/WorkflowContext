using Microsoft.Extensions.DependencyInjection;
using WorkflowContext.ConsoleApp;

await new ServiceCollection()
    .AddSingleton<TimeProvider>(_ => TimeProvider.System)
    .AddTransient<MessagePrinter>()
    .BuildServiceProvider()
    .GetRequiredService<MessagePrinter>()
    .PrintAsync();