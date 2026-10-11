namespace WorkflowContext;

/// <summary>
/// Creates a <typeparamref name="TSelf"/> from a <typeparamref name="TFrom"/>,
/// in the spirit of Rust's <see href="https://doc.rust-lang.org/std/convert/trait.From.html">From trait</see>.
/// </summary>
/// <typeparam name="TFrom">The source type.</typeparam>
/// <typeparam name="TSelf">The implementing type.</typeparam>
public interface IFrom<TFrom, TSelf>
    where TSelf : IFrom<TFrom, TSelf>
{
    /// <summary>
    /// Creates a <typeparamref name="TSelf"/> from <paramref name="source"/>.
    /// </summary>
    public static abstract TSelf From(TFrom source);
}
