# WorkflowContext

[![CI](https://github.com/lucaslenglet/WorkflowContext/actions/workflows/ci.yml/badge.svg)](https://github.com/lucaslenglet/WorkflowContext/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/lucaslgt.WorkflowContext)](https://www.nuget.org/packages/lucaslgt.WorkflowContext)
[![License: MIT-0](https://img.shields.io/badge/license-MIT--0-blue)](LICENSE)

Lightweight helpers to build and run workflow contexts and execution pipelines in .NET.

A `WorkflowContext<TData, TError>` carries your data, a success/failure state and the service
provider through a chain of synchronous or asynchronous steps. Small, composable operators keep the
pipeline readable and stop it at the first failure.

## Install

```
dotnet add package lucaslgt.WorkflowContext
```

Targets .NET 8 and .NET 10.

## Use

```csharp
using Microsoft.Extensions.DependencyInjection;
using WorkflowContext;

var services = new ServiceCollection().BuildServiceProvider();

var (data, state) = new WorkflowContext<string, string>(services, "hello")
    .Execute(ctx =>
    {
        Console.WriteLine(ctx.Data);
        return WorkflowState.Success();
    });

Console.WriteLine($"Success={state.IsSuccess}, Result: {data}");
```

With dependency injection, register the factory once, then inject `IWorkflowContextFactory`. Contexts it
creates resolve services from the current scope:

```csharp
services.AddWorkflowContext();

// In a scoped service
var context = factory.Create<Order, Error>(order, cancellationToken);
```

Steps resolve what they need from `ctx.Services`, which keeps them plain static methods, reusable across
workflows.

### Operators

| Operator       | Runs when             | Its result                                        | Exceptions                    |
|----------------|-----------------------|---------------------------------------------------|-------------------------------|
| `Execute`      | always                | applied, but never erases an earlier failure      | propagate                     |
| `ExecuteTry`   | always                | applied, but never erases an earlier failure      | become a failure (`TError.From`) |
| `IfSuccess`    | the state is success  | becomes the state                                 | propagate                     |
| `IfSuccessTry` | the state is success  | becomes the state                                 | become a failure (`TError.From`) |
| `OnFailure`    | the state is failure  | replaces the state: return a success to recover   | propagate                     |
| `OnFailureTry` | the state is failure  | replaces the state: return a success to recover   | become a failure (`TError.From`) |
| `IfSuccessAll` | the state is success  | runs every step, combines all their errors        | propagate                     |
| `IfSuccessAllTry` | the state is success | runs every step, combines all their errors       | become a failure (`TError.From`) |

`IfSuccess` and `IfSuccessTry` also take a `when` condition on the context, to make a step optional.
`OnFailure` and `OnFailureTry` take a `when` predicate on the error, to handle some failures and keep
the others as they are. `IfSuccessAll` is covered in [Multiple errors](#multiple-errors).

A failure is only ever replaced by `OnFailure`, so an `Execute` used for logging or cleanup in the middle
of a pipeline cannot hide an error.

Each operator accepts a synchronous or an asynchronous step, and can be chained on a
`Task<WorkflowContext<,>>`, so sync and async steps mix freely in one pipeline. The context is updated in
place: every operator returns the same instance.

The `Try` variants require the error type to implement `IFromException<TError>`, as `Error` does in
the examples below.

### Operators by example

The examples share this setup:

```csharp
record Error(string Message) : IFromException<Error>
{
    public static Error From(Exception exception) => new(exception.Message);
}

class Cart
{
    public int Items { get; set; }
}

WorkflowContext<Cart, Error> NewContext() => new(services, new Cart());

static WorkflowState<Error> AddItem(WorkflowContext<Cart, Error> ctx)
{
    ctx.Data.Items++;
    return WorkflowState.Success();
}

static WorkflowState<Error> ReadStock(WorkflowContext<Cart, Error> ctx) =>
    throw new TimeoutException("Stock service timed out.");
```

#### `IfSuccess`: stop at the first failure

```csharp
var result = NewContext()
    .IfSuccess(AddItem)                          // runs: Items = 1
    .IfSuccess(_ => new Error("Out of stock."))  // runs, and fails
    .IfSuccess(AddItem);                         // skipped: Items stays 1

// result.State.Error.Message == "Out of stock."
```

A step returns `WorkflowState.Success()`, or an error, which converts implicitly into a failure.

A `when` condition makes a step optional, and keeps that condition visible in the pipeline:

```csharp
var result = NewContext()
    .IfSuccess(
        when: ctx => ctx.Data.Items == 0,
        step: AddItem);        // runs only for an empty cart
```

#### `Execute`: always run, never hide a failure

```csharp
var result = NewContext()
    .IfSuccess(_ => new Error("Out of stock."))
    .Execute(ctx =>
    {
        Console.WriteLine($"Success: {ctx.State.IsSuccess}");  // runs: prints "Success: False"
        return WorkflowState.Success();                        // ignored: the context already failed
    });

// result.State.Error.Message == "Out of stock."
```

On a successful context, a failure returned by `Execute` is applied like any other.

#### `IfSuccessTry` / `ExecuteTry`: turn exceptions into failures

```csharp
var result = NewContext()
    .IfSuccessTry(ReadStock)   // throws: Error.From(exception) becomes the state
    .IfSuccess(AddItem);       // skipped

// result.State.Error.Message == "Stock service timed out."
```

Without `Try`, the exception propagates out of the pipeline.

#### `OnFailure`: recover, compensate or rewrite the error

```csharp
// Recover: return a success, and the pipeline resumes
var recovered = NewContext()
    .IfSuccessTry(ReadStock)
    .OnFailure(_ => WorkflowState.Success())
    .IfSuccess(AddItem);       // runs: Items = 1

// Compensate: undo what was done, and keep the error by returning the current state
var compensated = NewContext()
    .IfSuccess(AddItem)
    .IfSuccessTry(ReadStock)
    .OnFailure(ctx =>
    {
        ctx.Data.Items = 0;
        return ctx.State;
    });

// Rewrite the error
var rewritten = NewContext()
    .IfSuccessTry(ReadStock)
    .OnFailure(ctx => new Error($"Checkout failed: {ctx.State.Error.Message}"));
```

`OnFailure` is skipped on a successful context.

Handle only some errors with `when`; the others are kept untouched:

```csharp
var result = NewContext()
    .IfSuccessTry(ReadStock)
    .OnFailure(
        when: error => error.Message.Contains("timed out"),
        step: _ => WorkflowState.Success());   // only timeouts are recovered
```

When the recovery itself can throw, use `OnFailureTry`. If it throws, the error built from that
exception replaces the original one:

```csharp
static WorkflowState<Error> ReadCachedStock(WorkflowContext<Cart, Error> ctx) =>
    throw new IOException("Cache unavailable.");

var result = NewContext()
    .IfSuccessTry(ReadStock)
    .OnFailureTry(ReadCachedStock);

// result.State.Error.Message == "Cache unavailable."
```

To retry a failing operation, retry inside the step, with a library such as
[Polly](https://github.com/App-vNext/Polly): once a step has failed, the pipeline no longer knows which
step it was.

#### `MapError`: bring another error type into the workflow

```csharp
record PaymentError(string Reason);

static WorkflowState<PaymentError> Charge(decimal amount) => new PaymentError("Insufficient funds.");

var result = NewContext()
    .IfSuccess(_ => Charge(42m).MapError(error => new Error(error.Reason)));

// result.State.Error.Message == "Insufficient funds."
```

#### Mixing synchronous and asynchronous steps

```csharp
var result = await NewContext()
    .IfSuccess(AddItem)        // synchronous
    .IfSuccess(async ctx =>    // asynchronous: from here, the chain is a Task
    {
        await Task.Delay(10, ctx.CancellationToken);
        ctx.Data.Items++;
        return WorkflowState.Success();
    })
    .IfSuccess(AddItem);       // synchronous again, chained on the Task

// result.Data.Items == 3
```

#### Reading the outcome

```csharp
var (cart, state) = result;

var message = state.Match(
    onSuccess: () => $"{cart.Items} item(s) in the cart.",
    onFailure: error => error.Message);
```

### Multiple errors

A pipeline stops at the first failure. When several errors must be reported at once, typically during
validation, there are two ways.

A single step can return them all, since `TError` can be any type, a collection included:

```csharp
record Errors(IReadOnlyList<string> Messages);

static WorkflowState<Errors> Validate(WorkflowContext<Order, Errors> ctx)
{
    var messages = new List<string>();

    if (ctx.Data.Items == 0) messages.Add("The cart is empty.");
    if (ctx.Data.Card is null) messages.Add("The card is missing.");

    return messages.Count == 0 ? WorkflowState.Success() : new Errors(messages);
}
```

To keep the checks as separate steps, make the error type implement `ICombine<TError>`, and run the
steps with `IfSuccessAll`: every step runs, and the errors of those that fail are combined, in order.

```csharp
record Errors(IReadOnlyList<string> Messages) : ICombine<Errors>
{
    public Errors(string message) : this([message]) { }

    public static Errors Combine(Errors first, Errors second) => new([.. first.Messages, .. second.Messages]);
}

var result = context
    .IfSuccessAll(CheckCart, CheckCard, CheckAddress)   // all three run
    .IfSuccess(ReserveStock);                           // back to stopping at the first failure

// result.State.Error.Messages == ["The cart is empty.", "The card is missing."]
```

`IfSuccessAllTry` also turns the exception of a step into that step's error, and goes on with the next
steps.

### Cancellation

The context carries a `CancellationToken` for the steps to observe. The operators never check it
themselves. A step that throws `OperationCanceledException` under a `Try` operator goes through
`TError.From` like any other exception, so your error type decides how to represent it; use the
non-`Try` operators to let it propagate instead.

See [`samples/WorkflowContext.Sample`](samples/WorkflowContext.Sample) for a complete order workflow:
validation reporting every error at once, an optional coupon step, stock reservation, payment with
selective recovery and compensation, error mapping and cancellation.

## Repository layout

```
src/       WorkflowContext          the library, shipped as lucaslgt.WorkflowContext
samples/   WorkflowContext.Sample   an order workflow, played in five scenarios
tests/     WorkflowContext.Tests    unit tests
```

## Build and test

```
dotnet build
dotnet test
dotnet pack src/WorkflowContext -o artifacts
```

## Release

Push a tag `vMAJOR.MINOR.PATCH` (optionally with a `-suffix`). The release workflow builds, tests,
packs that version, publishes it to nuget.org through trusted publishing, then creates the matching
GitHub Release.

## Contributing

Pull requests are not accepted. Fork the repository to make your own version; see
[CONTRIBUTING.md](CONTRIBUTING.md). Report security vulnerabilities privately, as described in
[SECURITY.md](SECURITY.md).

## License

[MIT No Attribution (MIT-0)](LICENSE): use, copy, modify and redistribute freely, no attribution required.
