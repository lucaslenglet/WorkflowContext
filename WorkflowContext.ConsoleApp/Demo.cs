namespace WorkflowContext.ConsoleApp;

public class Demo(
    IServiceProvider serviceProvider,
    IWorkflowContextBuilder workflowContextBuilder)
{
    class Context : TimeSteps.IDate
    {
        public required Parity Parity { get; init; }
        public DateTime? Date { get; set; }
        public string? Message { get; set; }
    }

    enum Parity { Pair, Odd }

    record Error(string Message) : IFromException<Error>, IFrom<string, Error>
    {
        public static Error From(Exception exception) => new(exception.Message);
        public static Error From(string source) => new(source);
    }

    public async Task StartAsync()
    {
        // Initiate the context by yourself
        _ = new WorkflowContext<Context, Error>(serviceProvider, new Context
        {
            Parity = Parity.Odd
        });

        // Or initiate the context using the static WorkflowContextBuilder
        _ = WorkflowContextBuilder
            .Create(serviceProvider)
            .WithError<Error>()
            .WithData(new Context
            {
                Parity = Parity.Odd,
            })
            .Build();

        // Or initiate the context using an injected WorkflowContextBuilder
        var (data, state) = await workflowContextBuilder
            .WithError<Error>()
            .WithData(new Context
            {
                Parity = Parity.Pair,
            })
            .Build()

            // Shared
            .Execute(LogSteps.LogContext)

            // This method always returns an error WorkflowState<string>
            //.IfSuccess(ctx => ErrorSteps.IWillFailSaying(ctx, "Sorry, i had to fail...").Map(Error.From))

            // This method can thow, so ExecuteTry is required and Error must implement IFromException<Error>
            .ExecuteTry(CanThrow)

            // Local
            .IfSuccess(GetDateLocal)

            // Shared
            .IfSuccess(TimeSteps.GetDate)
            .IfSuccess(LogSteps.LogContext)

            // Local
            // This method can thow, so IfSuccessTry is required and Error must implement IFromException<Error>
            .IfSuccessTry(GetMessageLocal)

            // Shared
            .IfSuccess(LogSteps.LogContext);

        var message = await state
            .Match(
                onSuccess: () => Task.FromResult(data.Message!),
                onFailure: error => Task.FromResult(error.Message));

        Console.WriteLine(message);
    }

    static async Task<WorkflowState<Error>> GetDateLocal(WorkflowContext<Context, Error> ctx)
    {
        // Fake I/O call to demonstrate that steps can be asynchronous at any point
        await Task.CompletedTask;

        ctx.Data.Date = DateTime.Now;

        return WorkflowState.Success();
    }

    static WorkflowState<Error> GetMessageLocal(WorkflowContext<Context, Error> ctx)
    {
        var parity = ctx.Data.Parity == Parity.Odd ? 0 : 1;

        if (ctx.Data.Date!.Value.Second % 2 == parity)
        {
            // Either work as long as IfSuccessTry is used and Error implements IFromException<Error>
            throw new InvalidDataException($"Oops, {ctx.Data.Date.Value.Second} is not {ctx.Data.Parity}.");
            //return UnitResult.Failure(new Error($"Oops : {ctx.Value.Date.Value.Second}s"));
        }

        ctx.Data.Message = $"The date is {ctx.Data.Date.Value:G}.";

        return WorkflowState.Success();
    }

    static WorkflowState<Error> CanThrow(WorkflowContext<Context, Error> ctx)
    {
        if (Random.Shared.NextSingle() > 0.9)
        {
            throw new UnfortunateException();
        }

        return WorkflowState.Success();
    }

    class UnfortunateException() : Exception("That's unfortunate.");
}
