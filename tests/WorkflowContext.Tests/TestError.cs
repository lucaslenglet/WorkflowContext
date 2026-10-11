using System;
using System.Collections.Generic;

namespace WorkflowContext.Tests;

public sealed record TestError(string Message) : IFromException<TestError>
{
    public static TestError From(Exception exception) => new(exception.Message);
}

public sealed class TestData
{
    public int Counter { get; set; }
}

public sealed record TestErrors(IReadOnlyList<string> Messages) : ICombine<TestErrors>, IFromException<TestErrors>
{
    public TestErrors(string message)
        : this([message])
    {
    }

    public static TestErrors Combine(TestErrors first, TestErrors second) => new([.. first.Messages, .. second.Messages]);

    public static TestErrors From(Exception exception) => new(exception.Message);
}
