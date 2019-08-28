namespace CsharpMacros.Filters
{
    class CamelCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input) => input.ToCamelCase();
    }
}