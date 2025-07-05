using UnityEngine;
using System.Collections.Generic;

public static class LinqExtensions
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        System.Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
                yield return element;
        }
    }
}
