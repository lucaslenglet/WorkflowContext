using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WorkflowContext.ConsoleApp;

static class TimeSteps
{
    public static async Task GetDate<TData, TError>(WorkflowContext<TData, TError> context)
        where TData : IDate
    {
        // Fake I/O call to demonstrate that steps can be asynchronous at any point
        await Task.CompletedTask;

        var timeProvider = context.Services.GetRequiredService<TimeProvider>();

        context.Data.Date = timeProvider.GetLocalNow().DateTime;
    }

    public interface IDate
    {
        public DateTime? Date { get; set; }
    }
}

class LogSteps
{
    public static void LogContext<TData, TError>(WorkflowContext<TData, TError> context)
    {
        var logger = context.Services.GetRequiredService<ILogger<LogSteps>>();
        var scopedService = context.Services.GetRequiredService<ScopedService>();

        var json = JsonSerializer.Serialize(context);

        logger.LogInformation("Scope : {ScopedId}, Context: {Context}",
            scopedService.ScopeId, json);
    }
}