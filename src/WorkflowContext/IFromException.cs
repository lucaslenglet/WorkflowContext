using System;

namespace WorkflowContext;

/// <summary>
/// An error type that can be created from an exception. Required by the <c>Try</c> operators,
/// which turn any exception thrown by a step, <see cref="OperationCanceledException"/> included, into a failure.
/// </summary>
/// <typeparam name="TSelf">The implementing error type.</typeparam>
public interface IFromException<TSelf> : IFrom<Exception, TSelf>
    where TSelf : IFromException<TSelf>;
