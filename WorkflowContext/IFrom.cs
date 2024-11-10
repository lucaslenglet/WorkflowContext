namespace WorkflowContext;

public interface IFrom<TFrom, TTo>
    where TTo : IFrom<TFrom, TTo>
{
    public static abstract TTo From(TFrom source);
}