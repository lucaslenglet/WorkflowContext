using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WorkflowContext.Tests;

public class WorkflowContextExtensionsTests
{
    static WorkflowContext<TestData, TestError> NewContext() =>
        new(new ServiceCollection().BuildServiceProvider(), new TestData());

    static WorkflowState<TestError> Increment(WorkflowContext<TestData, TestError> ctx)
    {
        ctx.Data.Counter++;
        return WorkflowState.Success();
    }

    static async Task<WorkflowState<TestError>> IncrementAsync(WorkflowContext<TestData, TestError> ctx)
    {
        await Task.Yield();
        ctx.Data.Counter++;
        return WorkflowState.Success();
    }

    static WorkflowState<TestError> Fail(WorkflowContext<TestData, TestError> ctx) =>
        new TestError("failed");

    static WorkflowState<TestError> Throw(WorkflowContext<TestData, TestError> ctx) =>
        throw new InvalidOperationException("thrown");

    static async Task<WorkflowState<TestError>> ThrowAsync(WorkflowContext<TestData, TestError> ctx)
    {
        await Task.Yield();
        throw new InvalidOperationException("thrown");
    }

    [Fact]
    public async Task Execute_RunsEveryStep_SyncAndAsync()
    {
        var context = await NewContext()
            .Execute(Increment)
            .Execute(IncrementAsync)
            .Execute(Increment)
            .Execute(IncrementAsync);

        Assert.Equal(4, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task Execute_RunsAfterAFailure_WithoutErasingIt()
    {
        var context = await NewContext()
            .Execute(Fail)
            .Execute(Increment)
            .Execute(IncrementAsync);

        Assert.Equal(2, context.Data.Counter);
        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void Execute_KeepsTheFirstFailure()
    {
        var context = NewContext()
            .Execute(Fail)
            .Execute(_ => new TestError("second"));

        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void ExecuteTry_KeepsTheFirstFailure()
    {
        var context = NewContext()
            .Execute(Fail)
            .ExecuteTry(Throw);

        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void Execute_StoresTheReturnedFailure()
    {
        var context = NewContext().Execute(Fail);

        Assert.True(context.State.IsFailure);
        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void Execute_LetsExceptionsPropagate()
    {
        Assert.Throws<InvalidOperationException>(() => NewContext().Execute(Throw));
    }

    [Fact]
    public void ExecuteTry_ConvertsAnExceptionIntoAFailure()
    {
        var context = NewContext().ExecuteTry(Throw);

        Assert.True(context.State.IsFailure);
        Assert.Equal("thrown", context.State.Error.Message);
    }

    [Fact]
    public async Task ExecuteTry_ConvertsAnAsyncExceptionIntoAFailure()
    {
        var context = await NewContext().ExecuteTry(ThrowAsync);

        Assert.True(context.State.IsFailure);
        Assert.Equal("thrown", context.State.Error.Message);
    }

    [Fact]
    public async Task ExecuteTry_OnATask_ConvertsExceptionsIntoFailures()
    {
        var sync = await NewContext().Execute(IncrementAsync).ExecuteTry(Throw);
        var async = await NewContext().Execute(IncrementAsync).ExecuteTry(ThrowAsync);

        Assert.Equal("thrown", sync.State.Error.Message);
        Assert.Equal("thrown", async.State.Error.Message);
    }

    [Fact]
    public async Task IfSuccess_RunsTheSteps_WhileSuccessful()
    {
        var context = await NewContext()
            .IfSuccess(Increment)
            .IfSuccess(IncrementAsync)
            .IfSuccess(Increment)
            .IfSuccess(IncrementAsync);

        Assert.Equal(4, context.Data.Counter);
    }

    [Fact]
    public async Task IfSuccess_SkipsEveryStep_AfterAFailure()
    {
        var context = await NewContext()
            .Execute(Fail)
            .IfSuccess(Increment)
            .IfSuccess(IncrementAsync)
            .IfSuccess(Increment)
            .IfSuccess(IncrementAsync);

        Assert.Equal(0, context.Data.Counter);
        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public async Task IfSuccess_OnATask_SkipsAfterAnAsyncFailure()
    {
        var context = await NewContext()
            .Execute(IncrementAsync)
            .Execute(Fail)
            .IfSuccess(Increment)
            .IfSuccess(IncrementAsync);

        Assert.Equal(1, context.Data.Counter);
        Assert.True(context.State.IsFailure);
    }

    [Fact]
    public async Task IfSuccessTry_ConvertsExceptions_AndStopsThePipeline()
    {
        var context = await NewContext()
            .IfSuccessTry(Increment)
            .IfSuccessTry(ThrowAsync)
            .IfSuccessTry(Increment)
            .IfSuccess(IncrementAsync);

        Assert.Equal(1, context.Data.Counter);
        Assert.Equal("thrown", context.State.Error.Message);
    }

    [Fact]
    public void IfSuccessTry_SkipsTheStep_AfterAFailure()
    {
        var context = NewContext()
            .Execute(Fail)
            .IfSuccessTry(Throw);

        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void ExecuteTry_ConvertsCancellation_SoTheErrorTypeDecides()
    {
        static WorkflowState<TestError> Cancel(WorkflowContext<TestData, TestError> ctx) =>
            throw new OperationCanceledException("cancelled");

        var context = NewContext().ExecuteTry(Cancel);

        Assert.Equal("cancelled", context.State.Error.Message);
    }

    [Fact]
    public void OnFailure_IsSkipped_OnSuccess()
    {
        var context = NewContext().OnFailure(Increment);

        Assert.Equal(0, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task OnFailure_CanRecover_AndThePipelineResumes()
    {
        var context = await NewContext()
            .Execute(Fail)
            .IfSuccess(Increment)
            .OnFailure(IncrementAsync)
            .IfSuccess(Increment);

        Assert.Equal(2, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task OnFailure_CanReplaceTheError()
    {
        var context = await NewContext()
            .Execute(IncrementAsync)
            .Execute(Fail)
            .OnFailure(ctx => new TestError($"wrapped: {ctx.State.Error.Message}"));

        Assert.Equal("wrapped: failed", context.State.Error.Message);
    }

    [Fact]
    public async Task IfSuccess_When_RunsTheStep_OnlyIfTheConditionHolds()
    {
        var context = await NewContext()
            .IfSuccess(when: ctx => ctx.Data.Counter == 0, step: Increment)
            .IfSuccess(when: ctx => ctx.Data.Counter == 0, step: IncrementAsync)
            .IfSuccess(when: ctx => ctx.Data.Counter == 1, step: IncrementAsync);

        Assert.Equal(2, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public void IfSuccess_When_IsNotEvaluated_AfterAFailure()
    {
        var called = false;

        var context = NewContext()
            .Execute(Fail)
            .IfSuccess(
                when: ctx =>
                {
                    called = true;
                    return true;
                },
                step: Increment);

        Assert.False(called);
        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public async Task IfSuccessTry_When_ConvertsExceptions_WhenTheConditionHolds()
    {
        var skipped = NewContext().IfSuccessTry(when: _ => false, step: Throw);
        var ran = await NewContext().Execute(IncrementAsync).IfSuccessTry(when: _ => true, step: ThrowAsync);

        Assert.True(skipped.State.IsSuccess);
        Assert.Equal("thrown", ran.State.Error.Message);
    }

    [Fact]
    public async Task OnFailure_When_HandlesAMatchingError()
    {
        var context = await NewContext()
            .Execute(Fail)
            .OnFailure(error => error.Message == "failed", IncrementAsync);

        Assert.Equal(1, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task OnFailure_When_KeepsOtherErrors_WithoutRunningTheStep()
    {
        var context = await NewContext()
            .Execute(IncrementAsync)
            .Execute(Fail)
            .OnFailure(error => error.Message == "timeout", Increment);

        Assert.Equal(1, context.Data.Counter);
        Assert.Equal("failed", context.State.Error.Message);
    }

    [Fact]
    public void OnFailure_When_IsSkipped_OnSuccess_WithoutCallingThePredicate()
    {
        var called = false;

        var context = NewContext().OnFailure(
            error =>
            {
                called = true;
                return true;
            },
            Increment);

        Assert.False(called);
        Assert.Equal(0, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public void OnFailure_LetsExceptionsPropagate()
    {
        Assert.Throws<InvalidOperationException>(() => NewContext().Execute(Fail).OnFailure(Throw));
    }

    [Fact]
    public async Task OnFailureTry_CanRecover()
    {
        var context = await NewContext()
            .Execute(Fail)
            .OnFailureTry(IncrementAsync)
            .IfSuccess(Increment);

        Assert.Equal(2, context.Data.Counter);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task OnFailureTry_WhenTheRecoveryThrows_ItsErrorReplacesTheOriginal()
    {
        var sync = NewContext().Execute(Fail).OnFailureTry(Throw);
        var async = await NewContext().Execute(IncrementAsync).Execute(Fail).OnFailureTry(ThrowAsync);

        Assert.Equal("thrown", sync.State.Error.Message);
        Assert.Equal("thrown", async.State.Error.Message);
    }

    [Fact]
    public async Task OnFailureTry_IsSkipped_OnSuccess()
    {
        var context = await NewContext()
            .Execute(IncrementAsync)
            .OnFailureTry(Throw);

        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public async Task OnFailureTry_When_OnlyHandlesMatchingErrors()
    {
        var handled = await NewContext()
            .Execute(Fail)
            .OnFailureTry(error => error.Message == "failed", ThrowAsync);
        var kept = NewContext()
            .Execute(Fail)
            .OnFailureTry(error => error.Message == "timeout", Throw);

        Assert.Equal("thrown", handled.State.Error.Message);
        Assert.Equal("failed", kept.State.Error.Message);
    }

    [Fact]
    public async Task Pipeline_WithLambdas_InfersTheRightOverloads()
    {
        var context = await NewContext()
            .Execute(ctx => WorkflowState.Success())
            .IfSuccess(async ctx =>
            {
                await Task.Yield();
                ctx.Data.Counter = 42;
                return WorkflowState.Success();
            })
            .IfSuccessTry(ctx => ctx.Data.Counter == 42
                ? WorkflowState.Success()
                : new TestError("unexpected"));

        Assert.True(context.State.IsSuccess);
        Assert.Equal(42, context.Data.Counter);
    }
}
