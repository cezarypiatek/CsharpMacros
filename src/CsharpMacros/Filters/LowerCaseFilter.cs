namespace CsharpMacros.Filters
{
    class LowerCaseFilter : IPlaceholderFilter
    {
        public string Filter(string input)
        {
            return input.ToLower();
        }
    }
}