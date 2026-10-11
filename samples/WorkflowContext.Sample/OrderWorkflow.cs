using Microsoft.Extensions.DependencyInjection;

namespace WorkflowContext.Sample;

sealed class OrderWorkflow(IWorkflowContextFactory workflowContextFactory)
{
    public async Task<WorkflowState<OrderError>> PlaceAsync(Order order, CancellationToken cancellationToken)
    {
        var (_, state) = await workflowContextFactory
            .Create<Order, OrderError>(order, cancellationToken)

            // Every check runs, and all their errors are reported together
            .IfSuccessAll(CheckCart, CheckQuantities, CheckCard)

            // From here, the pipeline stops at the first failure
            // An optional step: the condition stays visible in the pipeline
            .IfSuccess(when: ctx => ctx.Data.Coupon is not null, step: ApplyCoupon)
            .IfSuccess(ReserveStock)

            // The gateway can throw: IfSuccessTry turns the exception into an OrderError
            .IfSuccessTry(ChargePayment)

            // Recovers from one kind of failure only: the queue can throw too, hence OnFailureTry
            .OnFailureTry(when: error => error.Code == "gateway-unavailable", step: QueuePayment)

            // Runs only on a failure that was not recovered: compensates, and keeps the error
            .OnFailure(ReleaseStock)

            .IfSuccess(SendConfirmation)

            // A shared step: always runs, and never hides a failure
            .Execute(AuditSteps.Record);

        return state;
    }

    static WorkflowState<OrderError> CheckCart(WorkflowContext<Order, OrderError> ctx) =>
        ctx.Data.Lines.Count == 0
            ? new OrderError("invalid-order", "The cart is empty.")
            : WorkflowState.Success();

    static WorkflowState<OrderError> CheckQuantities(WorkflowContext<Order, OrderError> ctx) =>
        ctx.Data.Lines.Any(line => line.Quantity <= 0)
            ? new OrderError("invalid-order", "Every quantity must be positive.")
            : WorkflowState.Success();

    static WorkflowState<OrderError> CheckCard(WorkflowContext<Order, OrderError> ctx) =>
        string.IsNullOrWhiteSpace(ctx.Data.Card)
            ? new OrderError("invalid-order", "The card is missing.")
            : WorkflowState.Success();

    static WorkflowState<OrderError> ApplyCoupon(WorkflowContext<Order, OrderError> ctx)
    {
        if (ctx.Data.Coupon != "WELCOME10")
        {
            return new OrderError("invalid-coupon", $"The coupon {ctx.Data.Coupon} does not exist.");
        }

        ctx.Data.Discount = Math.Round(ctx.Data.Total * 0.10m, 2);
        Console.WriteLine($"  [coupon] {ctx.Data.Coupon}: -{ctx.Data.Discount:0.00} EUR");

        return WorkflowState.Success();
    }

    static async Task<WorkflowState<OrderError>> ReserveStock(WorkflowContext<Order, OrderError> ctx)
    {
        var inventory = ctx.Services.GetRequiredService<IInventory>();

        ctx.Data.ReservationId = await inventory.ReserveAsync(ctx.Data.Lines);

        return WorkflowState.Success();
    }

    static async Task<WorkflowState<OrderError>> ChargePayment(WorkflowContext<Order, OrderError> ctx)
    {
        var gateway = ctx.Services.GetRequiredService<IPaymentGateway>();

        var payment = await gateway.ChargeAsync(ctx.Data.Card!, ctx.Data.Total, ctx.CancellationToken);

        return payment.MapError(OrderError.FromPayment);
    }

    static async Task<WorkflowState<OrderError>> QueuePayment(WorkflowContext<Order, OrderError> ctx)
    {
        await ctx.Services.GetRequiredService<IPaymentQueue>().EnqueueAsync(ctx.Data.Id, ctx.Data.Total);

        ctx.Data.PaymentPending = true;

        // A success recovers: the stock stays reserved and the pipeline resumes
        return WorkflowState.Success();
    }

    static async Task<WorkflowState<OrderError>> ReleaseStock(WorkflowContext<Order, OrderError> ctx)
    {
        if (ctx.Data.ReservationId is { } reservationId)
        {
            await ctx.Services.GetRequiredService<IInventory>().ReleaseAsync(reservationId);
        }

        // Returning the current state keeps the failure; returning a success would recover from it
        return ctx.State;
    }

    static WorkflowState<OrderError> SendConfirmation(WorkflowContext<Order, OrderError> ctx)
    {
        var payment = ctx.Data.PaymentPending
            ? $"payment of {ctx.Data.Total:0.00} EUR pending"
            : $"{ctx.Data.Total:0.00} EUR charged";

        ctx.Services
            .GetRequiredService<INotifier>()
            .Send($"Order {ctx.Data.Id} confirmed, {payment}.");

        return WorkflowState.Success();
    }
}

// Generic steps work with any workflow, whatever its data and error types
static class AuditSteps
{
    public static WorkflowState<TError> Record<TData, TError>(WorkflowContext<TData, TError> ctx)
    {
        var clock = ctx.Services.GetRequiredService<TimeProvider>();
        var outcome = ctx.State.Match(() => "succeeded", error => $"failed: {error}");

        Console.WriteLine($"  [audit {clock.GetLocalNow():HH:mm:ss}] {typeof(TData).Name} workflow {outcome}");

        return WorkflowState.Success();
    }
}
