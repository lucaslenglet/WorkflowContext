using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkflowContext;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Iter<T>(this IEnumerable<T> items)
    {
        var enumerator = items.GetEnumerator();
        while (enumerator.MoveNext());
        return items;
    }

    public static IEnumerable<T> Iter<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }
        return items;
    }

    public static async Task<IEnumerable<T>> Iter<T>(this IEnumerable<T> items, Func<T, Task> action)
    {
        foreach (var item in items)
        {
            await action(item);
        }
        return items;
    }
}