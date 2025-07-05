using System;
using System.Collections.Generic;
using System.Text;

namespace TD.Utilities
{
	public static class StringUtil
	{
        public static string GetRandomString(string[] inputs)
        {
            if(inputs == null || inputs.Length == 0)
                return string.Empty;

            int index = UnityEngine.Random.Range(0, inputs.Length);
            return inputs[index];
        }
        public static int Extract_TOP_Number(string input)
        {
            // Find the index of the underscore character
            int underscoreIndex = input.IndexOf('_');

            // Check if underscore exists and if it's not the last character
            if (underscoreIndex != -1 && underscoreIndex < input.Length - 1)
            {
                // Get the substring starting from the character after the underscore
                string numberString = input.Substring(underscoreIndex + 1);

                // Attempt to parse the substring to an integer
                if (int.TryParse(numberString, out int number))
                {
                    return number;
                }
            }

            // Return 0 if the number extraction fails
            return 0;
        }
        /// <summary>
        /// Extracts the desired name from the input string by removing a specified prefix or unwanted part.
        /// </summary>
        /// <param name="input">The full input string (e.g., "MWeapon_Katana").</param>
        /// <param name="removePart">The part to remove from the input string (e.g., "MWeapon_").</param>
        /// <returns>The extracted name after removing the unwanted part (e.g., "Katana").</returns>
        public static string ExtractName(string input, string removePart)
        {
            if (string.IsNullOrEmpty(input)) return "None"; // Return "None" if input is null or empty
            if (string.IsNullOrEmpty(removePart)) return input; // If no remove part is provided, return input as is

            return input.StartsWith(removePart) ? input.Substring(removePart.Length) : input;
        }
        /// <summary>
        /// Extracts the desired name from the input string by removing either a specific prefix or everything before the first "_".
        /// </summary>
        /// <param name="input">The full input string (e.g., "MWeapon_Katana").</param>
        /// <param name="prefix">The prefix to remove (if known). If empty or null, it removes everything before "_".</param>
        /// <returns>The extracted name after removing the prefix (e.g., "Katana").</returns>
        public static string ExtractNameWithPrefix(string input, string prefix)
        {
            if (string.IsNullOrEmpty(input)) return "None"; // Handle empty input

            // If a prefix is provided and matches, remove it
            if (!string.IsNullOrEmpty(prefix) && input.StartsWith(prefix))
            {
                return input.Substring(prefix.Length);
            }

            // If no prefix is provided, remove everything before and including the first "_"
            int underscoreIndex = input.IndexOf('_');
            return (underscoreIndex != -1) ? input.Substring(underscoreIndex + 1) : input;
        }

        //playerName#1282 => Extract to playerName
        public static string ExtractName(string inputString)
        {
            // Split the string by "#"
            string[] parts = inputString.Split('#');

            // If there is at least one part, return the first part (ahead of "#")
            if (parts.Length > 0)
            {
                return parts[0];
            }
            else
            {
                // Handle the case where there is no "#", return the whole string
                return inputString;
            }
        }

        public static List<string> ExtractNames(List<string> inputStrings)
        {
            List<string> extractedNames = new List<string>();

            foreach (string inputString in inputStrings)
            {
                // Split the string by "#"
                string[] parts = inputString.Split('#');

                // If there is at least one part, add the first part (ahead of "#") to the list
                if (parts.Length > 0)
                {
                    extractedNames.Add(parts[0]);
                }
                else
                {
                    // Handle the case where there is no "#", add the whole string
                    extractedNames.Add(inputString);
                }
            }

            return extractedNames;
        }

        public static string[] Split(string strValue, string splitValue)
		{
			return strValue.Split(new string[]{splitValue}, StringSplitOptions.None);
		}

		public static string SplitStringByMaxChars(string strValue, int maxChars)
		{
			const string splittedChars = "...";
			maxChars += splittedChars.Length;

			if ( strValue.Length+splittedChars.Length > maxChars )
			{
				strValue = strValue.Substring(0, maxChars-splittedChars.Length )+splittedChars;
			}
			return strValue;
		}

		public static string SplitStringByStartEnd (string strValue, string start, string end)
		{
			if (strValue.Contains(start))
			{
				string[] array = strValue.Split(new string[]{start}, StringSplitOptions.None);
				strValue = array[array.Length-1];
			}

			if (strValue.Contains(end))
			{
				strValue = strValue.Split(new string[]{end}, StringSplitOptions.None)[0];
			}

			return strValue;
		}

		public static string UppercaseFirstLetter(string value)
		{
			if (string.IsNullOrEmpty(value) || value.Length == 0)
				return value;
			
			if (value.Length == 1)
				return value.ToUpper();
			
			return string.Format("{0}{1}", value.Substring(0, 1).ToUpper(), value.Substring(1, value.Length-1).ToLower());
		}

        public static string LowercaseFirstLetter(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
                return value;

            if (value.Length == 1)
                return value.ToLower();

            return string.Format("{0}{1}", value.Substring(0, 1).ToLower(), value.Substring(1, value.Length - 1));
        }

		public static string ReplaceHtmlTags(string value)
		{
			string result = value.Replace("<", "\u3008");
			result = result.Replace(">", "\u3009");
			return result;
		}
		
		public static string PrepareTextForBubble(string text, int maxLines, int maxLineLength, string ellipsis)
        {
            string[] words = text.Split(' ');
            List<string> result = new List<string>();
            int index = 0, length = 0;
            bool wordCut = true;
            for (int i = 0; i <= maxLines; i++)
            {
                if (i == maxLines)
                {
                    if (!wordCut)
                    {
                        result.Add(" ");
                        length++;
                        while (true)
                        {
                            if (length == 0 || length + ellipsis.Length <= maxLineLength)
                            {
                                result.Add(ellipsis);
                                break;
                            }
                            length -= result[result.Count - 2].Length + 1;
                            result.RemoveRange(result.Count - 2, 2);
                        }
                    }
                    break;
                }
                if (i > 0)
                {
                    result.Add("\n");
                }
                length = 0;
                wordCut = false;
                while (index < words.Length)
                {
                    string word = words[index];
                    bool extraSpace = length > 0 && word.Length > 0;
                    if (length + (extraSpace ? 1 : 0) + word.Length <= maxLineLength)
                    {
                        if (extraSpace)
                        {
                            result.Add(" ");
                            length++;
                        }
                        result.Add(word);
                        length += word.Length;
                    }
                    else
                    {
                        break;
                    }
                    index++;
                }
                if (length == 0 && index < words.Length)
                {
                    result.Add(words[index].Substring(0, Math.Max(1, maxLineLength - ellipsis.Length)));
                    result.Add(ellipsis);
                    index++;
                    wordCut = true;
                }
                if (index == words.Length)
                {
                    break;
                }
            }
            return String.Join("", result.ToArray());
        }

	    private readonly static StringBuilder _stringBuilder = new StringBuilder();

        public static string Concat(params string[] texts)
        {
            _stringBuilder.Length = 0;

            for (int i = 0; i < texts.Length; i++)
            {
                _stringBuilder.Append(texts[i]);
            }
            return _stringBuilder.ToString();
        }
	}
}