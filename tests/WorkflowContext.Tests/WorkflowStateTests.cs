using System;
using Xunit;

namespace WorkflowContext.Tests;

public class WorkflowStateTests
{
    [Fact]
    public void Success_IsSuccess()
    {
        var state = WorkflowState.Success<TestError>();

        Assert.True(state.IsSuccess);
        Assert.False(state.IsFailure);
    }

    [Fact]
    public void Failure_IsFailure_AndCarriesError()
    {
        var error = new TestError("boom");

        var state = WorkflowState.Failure(error);

        Assert.True(state.IsFailure);
        Assert.False(state.IsSuccess);
        Assert.Same(error, state.Error);
    }

    [Fact]
    public void Default_IsSuccess()
    {
        WorkflowState<TestError> state = default;

        Assert.True(state.IsSuccess);
    }

    [Fact]
    public void Error_OnSuccess_Throws()
    {
        var state = WorkflowState.Success<TestError>();

        Assert.Throws<InvalidOperationException>(() => state.Error);
    }

    [Fact]
    public void Failure_WithNullError_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => WorkflowState.Failure<TestError>(null!));
    }

    [Fact]
    public void ImplicitConversion_FromError_IsFailure()
    {
        WorkflowState<TestError> state = new TestError("boom");

        Assert.True(state.IsFailure);
        Assert.Equal("boom", state.Error.Message);
    }

    [Fact]
    public void ImplicitConversion_FromNonGenericSuccess_IsSuccess()
    {
        WorkflowState<TestError> state = WorkflowState.Success();

        Assert.True(state.IsSuccess);
    }

    [Fact]
    public void MapError_OnFailure_MapsTheError()
    {
        var state = WorkflowState.Failure("boom");

        var mapped = state.MapError(message => new TestError(message));

        Assert.True(mapped.IsFailure);
        Assert.Equal("boom", mapped.Error.Message);
    }

    [Fact]
    public void MapError_OnSuccess_StaysSuccess_WithoutCallingTheMapper()
    {
        var state = WorkflowState.Success<string>();
        var called = false;

        var mapped = state.MapError(message =>
        {
            called = true;
            return new TestError(message);
        });

        Assert.True(mapped.IsSuccess);
        Assert.False(called);
    }

    [Fact]
    public void Match_CallsTheMatchingBranch()
    {
        Assert.Equal("ok", WorkflowState.Success<TestError>().Match(() => "ok", error => error.Message));
        Assert.Equal("boom", WorkflowState.Failure(new TestError("boom")).Match(() => "ok", error => error.Message));
    }

    [Fact]
    public void Match_WithActions_CallsTheMatchingBranch()
    {
        string? result = null;

        WorkflowState.Failure(new TestError("boom")).Match(
            onSuccess: () => result = "ok",
            onFailure: error => result = error.Message);

        Assert.Equal("boom", result);
    }
}
