namespace CsharpMacros.Filters
{
    class SnakeCaseFilter: IPlaceholderFilter
    {
        public string Filter(string input) => input.ToSnakeCase();
    }
}