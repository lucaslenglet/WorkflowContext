using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WorkflowContext.ConsoleApp;

static class TimeSteps
{
    public static async Task<WorkflowState<TError>> GetDate<TData, TError>(WorkflowContext<TData, TError> context)
        where TData : IDate
    {
        // Fake I/O call to demonstrate that steps can be asynchronous at any point
        await Task.CompletedTask;

        var timeProvider = context.Services.GetRequiredService<TimeProvider>();

        context.Data.Date = timeProvider.GetLocalNow().DateTime;

        return WorkflowState.Success();
    }

    public interface IDate
    {
        public DateTime? Date { get; set; }
    }
}

static class LogSteps
{
    public static WorkflowState<TError> LogContext<TData, TError>(WorkflowContext<TData, TError> context)
    {
        var logger = context.Services.GetRequiredService<ILogger<WorkflowContext<TData, TError>>>();

        var json = JsonSerializer.Serialize(context);

        logger.LogInformation("Context: {Context}", json);

        return WorkflowState.Success();
    }
}

static class ErrorSteps
{
    public static WorkflowState<string> IWillFailSaying<TData, TError>(WorkflowContext<TData, TError> context, string message) =>
        WorkflowState.Failure(message);
}