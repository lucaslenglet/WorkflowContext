namespace WorkflowContext;

/// <summary>
/// An error type that can merge two of its instances into one, for example by concatenating their messages.
/// Required by <c>IfSuccessAll</c>, which runs several steps and keeps the errors of all those that fail.
/// </summary>
/// <typeparam name="TSelf">The implementing error type.</typeparam>
public interface ICombine<TSelf>
    where TSelf : ICombine<TSelf>
{
    /// <summary>
    /// Merges <paramref name="first"/> and <paramref name="second"/>, keeping their order.
    /// </summary>
    public static abstract TSelf Combine(TSelf first, TSelf second);
}
