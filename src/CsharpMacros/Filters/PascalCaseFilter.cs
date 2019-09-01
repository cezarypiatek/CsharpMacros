namespace CsharpMacros.Filters
{
    class PascalCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input) => input.ToPascalCase();
    }
}