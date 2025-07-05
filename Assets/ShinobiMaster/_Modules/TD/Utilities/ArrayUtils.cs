using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Utilities
{
    public static class ArrayUtils
    {
        public static string ToStringWithDelimiter<T>(T[] array, string delimiter = ";")
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(delimiter, array);
        }
        public static void RemoveAt<T>(ref T[] values, int index)
        {
            if (index == values.Length - 1)
            {
                Array.Resize(ref values, index);
                return;
            }

            T[] result = new T[values.Length - 1];
            if (index != 0)
            {
                Array.Copy(values, 0, result, 0, index);
            }
            Array.Copy(values, index + 1, result, index, values.Length - index - 1);
            values = result;
        }

        public static void Remove<T>(ref T[] values, T value)
        {
            RemoveAt(ref values, Array.IndexOf(values, value));
        }

        public static void Add<T>(ref T[] values, T value)
        {
            Array.Resize(ref values, values.Length + 1);
            values[values.Length - 1] = value;
        }

        public static void AddRange<T>(ref T[] array, int count, IEnumerable<T> values)
        {
            int startCount = array.Length;
            Array.Resize(ref array, array.Length + count);

            foreach (var value in values)
            {
                array[startCount++] = value;
            }
        }
        public static T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                Debug.LogWarning("The list is empty or null!");
                return default;
            }

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
        public static List<string> Shuffle(List<string> tagList)
        {
            if (tagList == null || tagList.Count <= 1)
                return tagList;

            for (int i = tagList.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1); // Range is inclusive on both ends for Fisher-Yates
                (tagList[i], tagList[j]) = (tagList[j], tagList[i]); // Swap
            }

            return tagList;
        }

    }
}