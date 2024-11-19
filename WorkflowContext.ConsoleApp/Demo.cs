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
        public IEnumerable<int> Integers { get; set; } = Enumerable.Range(0, 10);
    }

    enum Parity { Pair, Odd }

    public record Error(string Message) : IFromException<Error>, IFrom<string, Error>
    {
        public static Error From(Exception exception) => new(exception.Message);
        public static Error From(string source) => new(source);
    }

    abstract class Planner : IWorkflowContextAsync<Context, Error>
    {
        public static WorkflowPlannerAsync<Context, Error> Plan => static ctx => ctx
            // Shared
            .Execute(LogSteps.LogContext)

            // This method always returns an error UnitResult<string>, but this is automatically mapped because Error implements IFrom<string, Error>
            //.IfSuccess(ctx => ErrorSteps.IWillFailSaying(ctx, "Sorry, i had to fail..."))

            // This method can thow, so ExecuteTry is required and Error must implement IFromException<Error>
            .ExecuteTry(CanThrow)

            .IfSuccess(ctx => (ctx.Data.Integers ?? [])
                .Iter(i => ctx
                    .ExecuteMap(_ => i * 2) // You can change workflow Data type
                    .MapError(e => "") // You can also change workflow  type
                    .ExecuteScoped(c => Console.WriteLine("Integer was : {0}", c.Data))))

            // Used to create a new Service Provider scope so that scoped services are reinstantiated when resolved inside this method
            .IfSuccessScoped(ctx => ctx
                // Local
                .IfSuccess(GetDateLocal)

                // Shared
                .IfSuccess(TimeSteps.GetDate)
                .IfSuccess(LogSteps.LogContext))

            // Local
            // This method can thow, so IfSuccessTry is required and Error must implement IFromException<Error>
            .IfSuccessTry(GetMessageLocal)

            // Shared
            .IfSuccess(LogSteps.LogContext);

        public static WorkflowPlanner<int, Error> Plan2 => static ctx => ctx
            .Execute(c => Console.WriteLine("Integer was : {0}", c.Data));
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
        var (data, result) = await workflowContextBuilder
            .WithError<Error>()
            .WithData(new Context
            {
                Parity = Parity.Odd,
            })
            .Build()

            .Execute(Planner.Plan);

        var message = result
            .Match(
                onSuccess: () => data.Message,
                onFailure: error => error.Message);

        Console.WriteLine(message);
    }

    static async Task<WorkflowResult<Error>> GetDateLocal(WorkflowContext<Context, Error> ctx)
    {
        // Fake I/O call to demonstrate that steps can be asynchronous at any point
        await Task.CompletedTask;

        ctx.Data.Date = DateTime.Now;

        return WorkflowResult.Success<Error>();
    }

    static WorkflowResult<Error> GetMessageLocal(WorkflowContext<Context, Error> ctx)
    {
        var parity = ctx.Data.Parity == Parity.Odd ? 1 : 0;

        if (ctx.Data.Date!.Value.Second % 2 == parity)
        {
            // Either work as long as IfSuccessTry is used and Error implements IFromException<Error>
            throw new InvalidDataException($"Oops, {ctx.Data.Date.Value.Second} is not {ctx.Data.Parity}.");
            //return UnitResult.Failure(new Error($"Oops : {ctx.Value.Date.Value.Second}s"));
        }

        ctx.Data.Message = $"The date is {ctx.Data.Date.Value:G}.";

        return WorkflowResult.Success<Error>();
    }

    static void CanThrow(WorkflowContext<Context, Error> ctx)
    {
        if (Random.Shared.NextSingle() > 0.9)
        {
            throw new UnfortunateException();
        }
    }

    class UnfortunateException() : Exception("That's unfortunate.");
}
