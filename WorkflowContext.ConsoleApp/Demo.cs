using CSharpFunctionalExtensions;

namespace WorkflowContext.ConsoleApp;

internal class Demo(IServiceProvider serviceProvider)
{
    public async Task StartAsync()
    {
        // Initiate context by yourself
        var context = new WorkflowContext<DemoContext, Error>(serviceProvider, new DemoContext
        {
            Parity = Parity.Odd
        });

        context = await context
            // Local
            //.IfSuccess(GetDateLocal)

            // Shared
            .Execute(LogSteps.LogContext)

            // Used to create a new Service Provider scope so that scoped services are reinstantiated when resolved
            .IfSuccessScoped(ctx => ctx
                .IfSuccess(TimeSteps.GetDate)
                .IfSuccess(LogSteps.LogContext))

            // Local
            .IfSuccess(GetMessageLocal)

            // Shared
            .IfSuccess(LogSteps.LogContext);

        var message = context.Result
            .Match(
                onSuccess: () => context.Value.Message,
                onFailure: error => error.Message);

        Console.WriteLine(message);
    }

    static async Task<UnitResult<Error>> GetDateLocal(WorkflowContext<DemoContext, Error> ctx)
    {
        // Fake I/O call to demonstrate that steps can be asynchronous
        await Task.CompletedTask;

        ctx.Value.Date = DateTime.Now;

        return UnitResult.Success<Error>();
    }

    static UnitResult<Error> GetMessageLocal(WorkflowContext<DemoContext, Error> ctx)
    {
        var parity = ctx.Value.Parity == Parity.Odd ? 1 : 0;

        if (ctx.Value.Date!.Value.Second % 2 == parity)
        {
            return UnitResult.Failure(new Error($"Oops : {ctx.Value.Date.Value.Second}s"));
        }

        ctx.Value.Message = $"The date is {ctx.Value.Date.Value:G}.";

        return UnitResult.Success<Error>();
    }

    class DemoContext : IDate
    {
        public required Parity Parity { get; init; }

        public DateTime? Date { get; set; }

        public string? Message { get; set; }
    }

    enum Parity { Pair, Odd }

    record Error(string Message);
}
