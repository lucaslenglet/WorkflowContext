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
            // Shared
            .Execute(LogSteps.LogContext)

            // This method can thow, so ExecuteTry is required and Error must implement IFromException<Error>
            .ExecuteTry(CanThrow)

            // Used to create a new Service Provider scope so that scoped services are reinstantiated when resolved inside this method
            .IfSuccessScoped(ctx => ctx
                // Local
                //.IfSuccess(GetDateLocal)

                // Shared
                .IfSuccess(TimeSteps.GetDate)
                .IfSuccess(LogSteps.LogContext))

            // Local
            // This method can thow, so IfSuccessTry is required and Error must implement IFromException<Error>
            .IfSuccessTry(GetMessageLocal)

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
        // Fake I/O call to demonstrate that steps can be asynchronous at any point
        await Task.CompletedTask;

        ctx.Value.Date = DateTime.Now;

        return UnitResult.Success<Error>();
    }

    static UnitResult<Error> GetMessageLocal(WorkflowContext<DemoContext, Error> ctx)
    {
        var parity = ctx.Value.Parity == Parity.Odd ? 1 : 0;

        if (ctx.Value.Date!.Value.Second % 2 == parity)
        {
            // Either work as long as IfSuccessTry is used and Error implements IFromException<Error>
            throw new InvalidDataException($"Oops, {ctx.Value.Date.Value.Second} is not {ctx.Value.Parity}.");
            //return UnitResult.Failure(new Error($"Oops : {ctx.Value.Date.Value.Second}s"));
        }

        ctx.Value.Message = $"The date is {ctx.Value.Date.Value:G}.";

        return UnitResult.Success<Error>();
    }

    static void CanThrow(WorkflowContext<DemoContext, Error> ctx)
    {
        if (Random.Shared.NextSingle() > 0.5)
        {
            throw new UnfortunateException();
        }
    }

    class UnfortunateException() : Exception("That's unfortunate.");

    class DemoContext : TimeSteps.IDate
    {
        public required Parity Parity { get; init; }

        public DateTime? Date { get; set; }

        public string? Message { get; set; }
    }

    enum Parity { Pair, Odd }

    record Error(string Message) : IFromException<Error>
    {
        public static Error FromException(Exception exception) =>
            new(exception.Message);
    }
}
