using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return double.TryParse(input, out double d);
        }
        public static bool IsBool(this string input)
        {
            string low = input.ToLower();
            if (low == "true" || low == "false") return true;
            return false;
        }
        public static bool ToBool(this string input)
        {
            string low = input.ToLower();
            if (low == "true") return true;
            return false;
        }
    }
}
