namespace WorkflowContext;

// Mimic Rust's From trait behavior (https://doc.rust-lang.org/std/convert/trait.From.html)
public interface IFrom<TFrom, TSelf>
    where TSelf : IFrom<TFrom, TSelf>
{
    public static abstract TSelf From(TFrom source);
}