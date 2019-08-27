namespace CsharpMacros.Filters
{
    class PascalCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input)
        {
            return char.ToUpperInvariant(input[0]) + input.Substring(1);
        }
    }
}