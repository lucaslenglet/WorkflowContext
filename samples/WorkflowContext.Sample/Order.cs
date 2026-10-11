namespace WorkflowContext.Sample;

sealed record OrderLine(string Product, int Quantity, decimal UnitPrice);

sealed class Order
{
    public required string Id { get; init; }
    public required IReadOnlyList<OrderLine> Lines { get; init; }
    public string? Card { get; init; }
    public string? Coupon { get; init; }

    public decimal Total => Lines.Sum(line => line.Quantity * line.UnitPrice) - Discount;

    // Filled in by the workflow as it goes
    public decimal Discount { get; set; }
    public string? ReservationId { get; set; }
    public bool PaymentPending { get; set; }
}

sealed record OrderError(string Code, IReadOnlyList<string> Messages)
    : IFromException<OrderError>, ICombine<OrderError>
{
    public OrderError(string code, string message)
        : this(code, [message])
    {
    }

    // Used by the Try operators: the error type decides how exceptions, cancellation included, are represented
    public static OrderError From(Exception exception) => exception switch
    {
        OperationCanceledException => new("cancelled", "The order was cancelled."),
        HttpRequestException => new("gateway-unavailable", exception.Message),
        _ => new("unexpected", exception.Message),
    };

    // Used by IfSuccessAll, to report every validation problem at once. The combined error keeps the first code.
    public static OrderError Combine(OrderError first, OrderError second) =>
        new(first.Code, [.. first.Messages, .. second.Messages]);

    // Used with MapError, to bring the payment module's own error type into the order workflow
    public static OrderError FromPayment(PaymentError error) => new("payment-declined", error.Reason);

    public override string ToString() => $"{string.Join(" ", Messages)} [{Code}]";
}
