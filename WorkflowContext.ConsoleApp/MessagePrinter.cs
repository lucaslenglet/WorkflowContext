using CSharpFunctionalExtensions;

namespace WorkflowContext.ConsoleApp;

internal class MessagePrinter(IServiceProvider serviceProvider)
{
    public async Task PrintAsync()
    {
        // Initiate context by yourself
        var context = new MonContext(serviceProvider)
        {
            Parite = Parite.Impair
        };

        context = await context
            // Local
            //.Then(GetDateLocal)

            // Used to create a new Service Provider scope so that scoped services are reinstantiated when resolved
            .Scoped<MonContext, Erreur>(async ctx =>
            {
                // Requires external dependencies via the IServiceProvider
                await ctx
                    .Then(TimeSteps.GetDateUsingDependencies<MonContext, Erreur>);
            })            

            // Has no external dependency
            //.Then(TimeSteps.GetDateWithoutServiceProvider<Erreur>)

            // Local
            .Then(GetMessageLocal);

        var message = context.Result.Match(
            context.Message.GetValueOrDefault,
            erreur => erreur.Message);

        Console.WriteLine(message);
    }

    static async Task<UnitResult<Erreur>> GetDateLocal(MonContext ctx)
    {
        // Fake I/O call to demonstrate that steps can be asynchronous
        await Task.CompletedTask;

        ctx.Now = DateTime.Now;

        return UnitResult.Success<Erreur>();
    }

    static UnitResult<Erreur> GetMessageLocal(MonContext ctx)
    {
        var parite = ctx.Parite == Parite.Impair ? 1 : 0;

        if (ctx.Now.Value.Second % 2 == parite)
        {
            return UnitResult.Failure(new Erreur($"Mince : {ctx.Now.Value.Second}"));
        }

        ctx.Message = $"La date est {ctx.Now.Value:G}.";

        return UnitResult.Success<Erreur>();
    }

    class MonContext(IServiceProvider serviceProvider) : BaseContext<Erreur>(serviceProvider), INow
    {
        public required Parite Parite { get; init; }

        public Maybe<DateTime> Now { get; set; }

        public Maybe<string> Message { get; set; }
    }

    enum Parite { Pair, Impair }

    record Erreur(string Message);
}
