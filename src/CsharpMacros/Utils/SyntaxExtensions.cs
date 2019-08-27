using Microsoft.CodeAnalysis;

namespace CsharpMacros
{
    internal static class SyntaxExtensions
    {
        public static T FirstAncestorOfType<T>(this SyntaxNode token) where T : class
        {
            if (token is T)
            {
                return token as T;
            }

            if (token.Parent != null)
            {
                return FirstAncestorOfType<T>(token.Parent);
            }

            return default;
        }
    }
}