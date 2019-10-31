using Microsoft.CodeAnalysis;

namespace CsharpMacros
{
    public interface ICsharpMacroContext
    {
        SemanticModel SemanticModel { get; }
        Solution Solution { get; }

        Location MacroLocation { get; }
    }
}