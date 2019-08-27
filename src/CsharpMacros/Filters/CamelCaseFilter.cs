namespace CsharpMacros.Filters
{
    class CamelCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input)
        {
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }
    }
}