namespace CsharpMacros.Filters
{
    class UpperCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input) => input.ToUpperInvariant();
    }
}