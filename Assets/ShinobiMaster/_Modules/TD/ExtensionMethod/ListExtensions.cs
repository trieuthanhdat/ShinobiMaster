using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    private static Random rng = new Random();

    /// <summary>  Resize a List </summary>
    public static void Resize<T>(this List<T> list, int size, T element = default(T))
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            if (size > list.Capacity)   // Optimization
                list.Capacity = size;

            list.AddRange(Enumerable.Repeat(element, size - count));
        }
    }
    /// <summary>
    /// Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm.
    /// This method modifies the input list in-place, ensuring each permutation is equally likely, and returns the list for method chaining.
    /// Reference: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    /// </summary>
    /// <param name="list">The list to be shuffled.</param>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <returns>The shuffled list.</returns>
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (rng == null) rng = new Random();
        int count = list.Count;
        while (count > 1)
        {
            --count;
            int index = rng.Next(count + 1);
            (list[index], list[count]) = (list[count], list[index]);
        }

        return list;
    }

}
