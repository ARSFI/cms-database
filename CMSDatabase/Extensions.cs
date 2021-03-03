using System;
using System.Collections.Generic;
using System.Linq;

namespace winlink.cms.data
{
    internal static class Extensions
    {
        /// <summary>
        /// Convert delimited string to string list, removing blank elements and trimming each item.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiters">String list of characters that are used to separate string elements.</param>
        /// <returns></returns>
        public static List<string> DelimitedStringToList(this string input, string delimiters = ",")
        {
            //convert to list and remove empty elements
            var list = input.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            return list;
        }
    }
}
