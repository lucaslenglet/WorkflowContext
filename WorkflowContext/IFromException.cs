using System;

namespace WorkflowContext;

public interface IFromException<TError>
    where TError : IFromException<TError>
{
    public static abstract TError FromException(Exception exception);
}
