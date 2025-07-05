using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TD.Utilities
{
    public static class StringFormatUtils
    {
        /// <summary>
        /// Formats a string with zero-padding up to a specified length.
        /// </summary>
        /// <param name="input">The input number to format.</param>
        /// <param name="length">The total length of the output string (including zero-padding).</param>
        /// <returns>A zero-padded string.</returns>
        public static string ZeroPad(int input, int length)
        {
            return input.ToString().PadLeft(length, '0');
        }

        /// <summary>
        /// Formats a string by appending a prefix and/or suffix.
        /// </summary>
        /// <param name="input">The base string to format.</param>
        /// <param name="prefix">Prefix to add at the start (optional).</param>
        /// <param name="suffix">Suffix to add at the end (optional).</param>
        /// <returns>The formatted string.</returns>
        public static string AddPrefixSuffix(string input, string prefix = "", string suffix = "")
        {
            return $"{prefix}{input}{suffix}";
        }

        /// <summary>
        /// Conditionally formats a string based on a specified condition.
        /// </summary>
        /// <param name="input">The base string to format.</param>
        /// <param name="condition">Condition to check.</param>
        /// <param name="ifTrueFormat">Format string to apply if condition is true.</param>
        /// <param name="ifFalseFormat">Format string to apply if condition is false.</param>
        /// <returns>Conditionally formatted string.</returns>
        public static string ConditionalFormat(string input, bool condition, string ifTrueFormat, string ifFalseFormat)
        {
            return condition ? string.Format(ifTrueFormat, input) : string.Format(ifFalseFormat, input);
        }

        /// <summary>
        /// Trims a string to a specified length and optionally adds ellipsis at the end.
        /// </summary>
        /// <param name="input">The input string to trim.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <param name="addEllipsis">Whether to add ellipsis if trimmed.</param>
        /// <returns>The trimmed string.</returns>
        public static string TrimToLength(string input, int maxLength, bool addEllipsis = false)
        {
            if (input.Length <= maxLength) return input;
            return addEllipsis ? input.Substring(0, maxLength) + "..." : input.Substring(0, maxLength);
        }

        /// <summary>
        /// Replaces placeholders in the input string with specified values.
        /// </summary>
        /// <param name="input">The input format string containing placeholders (e.g., {0}, {1}).</param>
        /// <param name="args">The arguments to replace placeholders.</param>
        /// <returns>The formatted string.</returns>
        public static string ReplacePlaceholders(string input, params object[] args)
        {
            return string.Format(input, args);
        }

        /// <summary>
        /// Converts a camelCase or PascalCase string to a more readable format with spaces.
        /// </summary>
        /// <param name="input">The camelCase or PascalCase string.</param>
        /// <returns>A more readable string with spaces.</returns>
        public static string ToReadableString(string input)
        {
            return Regex.Replace(input, "(\\B[A-Z])", " $1");
        }

        /// <summary>
        /// Formats a string to title case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string formatted in title case.</returns>
        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }

        /// <summary>
        /// Safely truncates a string to a specific length without breaking words.
        /// </summary>
        /// <param name="input">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <returns>The truncated string.</returns>
        public static string TruncateWithoutBreakingWords(string input, int maxLength)
        {
            if (input.Length <= maxLength) return input;
            int lastSpace = input.LastIndexOf(' ', maxLength);
            return lastSpace > 0 ? input.Substring(0, lastSpace) + "..." : input.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// Formats a string by replacing placeholders and applying prefix and suffix.
        /// </summary>
        /// <param name="inputFormat">The input format string.</param>
        /// <param name="prefix">Optional prefix.</param>
        /// <param name="suffix">Optional suffix.</param>
        /// <param name="args">The arguments to replace placeholders.</param>
        /// <returns>The formatted string with prefix, placeholders replaced, and suffix.</returns>
        public static string FormatWithPrefixSuffix(string inputFormat, string prefix = "", string suffix = "", params object[] args)
        {
            var formattedString = string.Format(inputFormat, args);
            return $"{prefix}{formattedString}{suffix}";
        }
    }

}
