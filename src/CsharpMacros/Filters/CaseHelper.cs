using System.Linq;
using System.Text.RegularExpressions;

namespace CsharpMacros.Filters
{
    public static class CaseHelper
    {
        private static readonly Regex SplitPattern = new Regex(@"\W+");

        public static string ToPascalCase(this string input)
        {
            return SplitPattern.Split(input).Aggregate("", (left, right) => left + right.Trim().ToLowerInvariant().FirstLetterUp());
        }
        public static string ToCamelCase(this string input)
        {
            return input.ToPascalCase().FirstLetterDown();
        }
        public static string FirstLetterDown(this string input)
        {
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }
        public static string FirstLetterUp(this string input)
        {
            return char.ToUpperInvariant(input[0]) + input.Substring(1);
            
        }
    }
}