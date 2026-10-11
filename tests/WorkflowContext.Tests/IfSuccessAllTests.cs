using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WorkflowContext.Tests;

public class IfSuccessAllTests
{
    static WorkflowContext<TestData, TestErrors> NewContext() =>
        new(new ServiceCollection().BuildServiceProvider(), new TestData());

    static WorkflowState<TestErrors> Increment(WorkflowContext<TestData, TestErrors> ctx)
    {
        ctx.Data.Counter++;
        return WorkflowState.Success();
    }

    static async Task<WorkflowState<TestErrors>> IncrementAsync(WorkflowContext<TestData, TestErrors> ctx)
    {
        await Task.Yield();
        ctx.Data.Counter++;
        return WorkflowState.Success();
    }

    static WorkflowState<TestErrors> FailA(WorkflowContext<TestData, TestErrors> ctx) => new TestErrors("a");

    static WorkflowState<TestErrors> FailB(WorkflowContext<TestData, TestErrors> ctx) => new TestErrors("b");

    static async Task<WorkflowState<TestErrors>> FailAAsync(WorkflowContext<TestData, TestErrors> ctx)
    {
        await Task.Yield();
        return new TestErrors("a");
    }

    static async Task<WorkflowState<TestErrors>> FailBAsync(WorkflowContext<TestData, TestErrors> ctx)
    {
        await Task.Yield();
        return new TestErrors("b");
    }

    static WorkflowState<TestErrors> Throw(WorkflowContext<TestData, TestErrors> ctx) =>
        throw new InvalidOperationException("thrown");

    [Fact]
    public void IfSuccessAll_RunsEveryStep_AndCombinesTheErrorsInOrder()
    {
        var context = NewContext().IfSuccessAll(FailA, Increment, FailB, Increment);

        Assert.Equal(2, context.Data.Counter);
        Assert.Equal(["a", "b"], context.State.Error.Messages);
    }

    [Fact]
    public void IfSuccessAll_SucceedsWhenEveryStepSucceeds()
    {
        var context = NewContext().IfSuccessAll(Increment, Increment);

        Assert.Equal(2, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task IfSuccessAll_Async_AndOnATask()
    {
        var context = await NewContext()
            .Execute(IncrementAsync)
            .IfSuccessAll(FailAAsync, IncrementAsync, FailBAsync);

        Assert.Equal(2, context.Data.Counter);
        Assert.Equal(["a", "b"], context.State.Error.Messages);
    }

    [Fact]
    public async Task IfSuccessAll_IsSkipped_AfterAFailure()
    {
        var context = await NewContext()
            .Execute(FailA)
            .IfSuccessAll(Increment, FailB)
            .IfSuccessAll(IncrementAsync);

        Assert.Equal(0, context.Data.Counter);
        Assert.Equal(["a"], context.State.Error.Messages);
    }

    [Fact]
    public void IfSuccessAll_LetsExceptionsPropagate_AndStops()
    {
        var context = NewContext();

        Assert.Throws<InvalidOperationException>(() => context.IfSuccessAll(Throw, Increment));
        Assert.Equal(0, context.Data.Counter);
    }

    [Fact]
    public async Task IfSuccessAllTry_TurnsExceptionsIntoErrors_AndKeepsGoing()
    {
        var sync = NewContext().IfSuccessAllTry(FailA, Throw, Increment, FailB);
        var async = await NewContext().Execute(IncrementAsync).IfSuccessAllTry(Throw, FailB);

        Assert.Equal(1, sync.Data.Counter);
        Assert.Equal(["a", "thrown", "b"], sync.State.Error.Messages);
        Assert.Equal(["thrown", "b"], async.State.Error.Messages);
    }

    [Fact]
    public void IfSuccessAll_AfterIt_ThePipelineShortCircuitsAgain()
    {
        var context = NewContext()
            .IfSuccessAll(FailA, FailB)
            .IfSuccess(Increment);

        Assert.Equal(0, context.Data.Counter);
    }
}
