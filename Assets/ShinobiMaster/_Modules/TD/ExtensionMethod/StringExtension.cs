using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtension 
{
    public static List<T> FromJsonToList<T>(this string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        if (wrapper == null || wrapper.items == null) return null;
        return wrapper.items;
    }
    public static string RemoveSpecialCharacters(this string str)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    [System.Serializable]
    public class Wrapper<T>
    {
        public List<T> items;
    }
}
