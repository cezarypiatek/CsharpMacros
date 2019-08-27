namespace CsharpMacros.Filters
{
    class UpperCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input)
        {
            return input.ToUpperInvariant();
        }
    }
}