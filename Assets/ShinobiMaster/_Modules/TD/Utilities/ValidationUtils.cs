using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TD.Utilities
{
    public static class ValidationUtils
    {
        //Validate email format
        public static bool IsValidEmail(string email)
        {
            if(string.IsNullOrEmpty(email)) return false;

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(emailRegex, email);
        }

        //Valid password with specific rule (e.g minlength)
        public static bool IsValidPassword(string password, int minlength = 8)
        {
            if(string.IsNullOrEmpty(password)) return false;
            return password.Length >= minlength;
        }

        public static bool IsAdvancedPasswordValid(string password, int minLegnth = 8, bool requiredSpecialChar = true, bool requiredNumber = true, string? username = null)
        {
            if (string.IsNullOrEmpty(password)) return false;

            //Check for min length
            if(password.Length < minLegnth) return false;

            //Check for special character
            var specialChar = @"[!@#$%^&*(),./;'[]\<>?:\|{}]";
            if(requiredSpecialChar && !MatchPattern(password, specialChar))
                return false;

            //Check for number digit
            if(requiredNumber && !ContainDigits(password))
                return false;

            //Check if password contains username (case-insensitivity)
            if(!string.IsNullOrWhiteSpace(username) && password.IndexOf(username, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            return true;
        }

        public static bool IsNumberWithinRange(string input, int min, int max)
        {
            if(!int.TryParse(input, out int number)) 
                return false;
            return number >= min && number <= max;
        }

        public static bool IsValidText(string input, int minLength = 1)
        {
            return string.IsNullOrWhiteSpace(input) && input.Length >= minLength;
        }

        public static bool MatchPattern(string input, string pattern)
        {
            if(string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsDateWithinRange(DateTime input, DateTime start, DateTime end)
        {
            return input >= start && input <= end;
        }

        public static bool IsAlphabetic(string input)
        {
            return MatchPattern(input, @"^[a-zA-z]+$");
        }

        public static bool IsAlphaNumeric(string input)
        {
            return MatchPattern(input, @"^[a-zA-z0-9]+$");
        }

        public static bool ContainUpperCase(string input)
        {
            return MatchPattern(input, @"[A-Z]");
        }
        public static bool ContainLowerCase(string input)
        {
            return MatchPattern(input, @"[a-z]");
        }

        public static bool ContainDigits(string input)
        {
            return MatchPattern(input, @"\d");
        }
    }

}
