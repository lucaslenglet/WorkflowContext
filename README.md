# WorkflowContext

Lightweight helpers to build and run workflow contexts and execution pipelines in .NET.

## What it solves
- Makes it easy to create a context object that carries data and an execution state through a chain of synchronous or asynchronous steps.
- Provides small, composable operators (`Execute`, `ExecuteTry`, `IfSuccess`, `IfSuccessTry`, etc.) to build readable pipelines.

## Very small example
```csharp
var services = new ServiceCollection().BuildServiceProvider();

var ctx = WorkflowContextBuilder
	.Create(services)
	.WithError<string>()
	.WithData("hello")
	.Build()
	.Execute(c =>
    { 
        Console.WriteLine(c.Data);
        return WorkflowState.Success();
    });

var (data, state) = ctx;
Console.WriteLine($"Success={state.IsSuccess}, Result: {data}");
```

That's it — adapt the example types and steps to your needs.