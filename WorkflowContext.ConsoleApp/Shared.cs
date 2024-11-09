using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WorkflowContext.ConsoleApp;

static class TimeSteps
{
    public static async Task<UnitResult<TError>> GetDate<TValue, TError>(WorkflowContext<TValue, TError> context)
        where TValue : IDate
    {
        // Fake I/O call to demonstrate that steps can be asynchronous
        await Task.CompletedTask;

        var timeProvider = context.Services.GetRequiredService<TimeProvider>();

        context.Value.Date = timeProvider.GetLocalNow().DateTime;

        return UnitResult.Success<TError>();
    }
}

interface IDate
{
    public DateTime? Date { get; set; }
}

class LogSteps
{
    public static UnitResult<TError> LogContext<TValue, TError>(WorkflowContext<TValue, TError> context)
    {
        var logger = context.Services.GetRequiredService<ILogger<LogSteps>>();
        var scopedService = context.Services.GetRequiredService<ScopedService>();

        var json = JsonSerializer.Serialize(context);

        logger.LogInformation("Scope : {ScopedId}, Context: {Context}",
            scopedService.ScopeId, json);

        return UnitResult.Success<TError>();
    }
}