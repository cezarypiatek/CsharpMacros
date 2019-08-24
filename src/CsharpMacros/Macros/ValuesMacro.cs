using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CsharpMacros.Macros
{
    class ValuesMacro:ICsharpMacro
    {
        private readonly Regex tuplePattern = new Regex("(\\(.+?\\))(?:,|$)", RegexOptions.Compiled);
        private readonly Regex singlePattern = new Regex("(.+?)(?:,|$)", RegexOptions.Compiled);
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var tupleMatches = tuplePattern.Matches(param);
            if (tupleMatches.Count > 0)
            {
                return tupleMatches.OfType<Match>().Select(m => GetAttributesForTuple(m.Groups[1].Value));
            }

            return GetAttributesForSingleValue(param);
        }

        private IEnumerable<Dictionary<string, string>> GetAttributesForSingleValue(string param)
        {
            var singleMatches = singlePattern.Matches(param);
            foreach (Match match in singleMatches)
            {
                yield return new Dictionary<string, string>()
                {
                    ["value"] = match.Groups[1].Value.Trim()
                };
            }
        }

        private Dictionary<string, string> GetAttributesForTuple(string tupleValue)
        {
            var singleMatches = singlePattern.Matches(tupleValue.Trim('(', ')'));
            var attributes = new Dictionary<string, string>();
            var index = 1;
            foreach (Match match in singleMatches)
            {
                attributes.Add($"value{index}", match.Groups[1].Value.Trim());
                index++;
            }

            return attributes;
        }
    }
}
