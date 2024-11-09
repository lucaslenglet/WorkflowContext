using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext.ConsoleApp;

static class TimeSteps
{
    // Requires external dependencies via the IServiceProvider
    public static async Task<UnitResult<TError>> GetDateUsingDependencies<TContext, TError>(TContext context)
        where TContext : BaseContext<TError>, INow
    {
        // Fake I/O call to demonstrate that steps can be asynchronous
        await Task.CompletedTask;

        var timeProvider = context.Services.GetRequiredService<TimeProvider>();

        context.Now = timeProvider.GetLocalNow().DateTime;

        return UnitResult.Success<TError>();
    }

    // Has no external dependency
    public static UnitResult<TError> GetDateWithoutServiceProvider<TError>(INow context)
    {
        context.Now = new DateTime(2024, 9, 11, 20, 00, 23);

        return UnitResult.Success<TError>();
    }
}

interface INow
{
    public Maybe<DateTime> Now { get; set; }
}