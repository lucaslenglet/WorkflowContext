namespace WorkflowContext.Sample;

// Fake infrastructure, registered in the container and resolved by the steps through ctx.Services

interface IInventory
{
    Task<string> ReserveAsync(IReadOnlyList<OrderLine> lines);
    Task ReleaseAsync(string reservationId);
}

sealed class FakeInventory : IInventory
{
    public async Task<string> ReserveAsync(IReadOnlyList<OrderLine> lines)
    {
        await Task.Yield();
        var reservationId = $"R-{Guid.NewGuid().ToString()[..4]}";
        Console.WriteLine($"  [inventory] reserved {lines.Sum(line => line.Quantity)} item(s) ({reservationId})");
        return reservationId;
    }

    public async Task ReleaseAsync(string reservationId)
    {
        await Task.Yield();
        Console.WriteLine($"  [inventory] released {reservationId}");
    }
}

// A module with its own error type, mapped into OrderError by the workflow
sealed record PaymentError(string Reason);

interface IPaymentGateway
{
    Task<WorkflowState<PaymentError>> ChargeAsync(string card, decimal amount, CancellationToken cancellationToken);
}

sealed class FakePaymentGateway : IPaymentGateway
{
    // Test cards: 0000 is declined, 9999 makes the gateway throw, anything else is charged
    public async Task<WorkflowState<PaymentError>> ChargeAsync(string card, decimal amount, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        if (card == "9999")
        {
            throw new HttpRequestException("Payment gateway unreachable.");
        }

        if (card == "0000")
        {
            return new PaymentError("Insufficient funds.");
        }

        Console.WriteLine($"  [payment] charged {amount:0.00} EUR");
        return WorkflowState.Success();
    }
}

// Payments that could not go through are queued, to be retried later by another process
interface IPaymentQueue
{
    Task EnqueueAsync(string orderId, decimal amount);
}

sealed class FakePaymentQueue : IPaymentQueue
{
    public async Task EnqueueAsync(string orderId, decimal amount)
    {
        await Task.Yield();
        Console.WriteLine($"  [payment-queue] {orderId}: {amount:0.00} EUR queued for a later attempt");
    }
}

interface INotifier
{
    void Send(string message);
}

sealed class ConsoleNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"  [mail] {message}");
}
