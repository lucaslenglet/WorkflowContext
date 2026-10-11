using Microsoft.Extensions.DependencyInjection;
using WorkflowContext;
using WorkflowContext.Sample;

await using var services = new ServiceCollection()
    .AddSingleton(TimeProvider.System)
    .AddSingleton<IInventory, FakeInventory>()
    .AddSingleton<IPaymentGateway, FakePaymentGateway>()
    .AddSingleton<IPaymentQueue, FakePaymentQueue>()
    .AddSingleton<INotifier, ConsoleNotifier>()
    .AddScoped<OrderWorkflow>()
    .AddWorkflowContext()
    .BuildServiceProvider(validateScopes: true);

OrderLine[] cart = [new("Keyboard", 1, 49.90m), new("Mouse", 2, 19.90m)];

await PlaceOrder("Valid order, with a coupon", new() { Id = "A-001", Lines = cart, Card = "4242", Coupon = "WELCOME10" });
await PlaceOrder("Invalid order", new() { Id = "A-002", Lines = [new("Mouse", 0, 19.90m)], Card = "" });
await PlaceOrder("Card declined", new() { Id = "A-003", Lines = cart, Card = "0000" });
await PlaceOrder("Payment gateway down", new() { Id = "A-004", Lines = cart, Card = "9999" });

using var cancellation = new CancellationTokenSource();
cancellation.Cancel();
await PlaceOrder("Cancelled by the customer", new() { Id = "A-005", Lines = cart, Card = "4242" }, cancellation.Token);

async Task PlaceOrder(string scenario, Order order, CancellationToken cancellationToken = default)
{
    Console.WriteLine($"== {scenario}");

    // One scope per order, as a web request would have
    await using var scope = services.CreateAsyncScope();

    var state = await scope.ServiceProvider
        .GetRequiredService<OrderWorkflow>()
        .PlaceAsync(order, cancellationToken);

    Console.WriteLine(state.Match(
        onSuccess: () => $"=> {order.Id} placed{(order.PaymentPending ? ", payment pending" : "")}",
        onFailure: error => $"=> {order.Id} rejected: {error}"));
    Console.WriteLine();
}
