using Microsoft.CodeAnalysis;

namespace CsharpMacros
{
    class CsharpMacroContext : ICsharpMacroContext
    {
        public SemanticModel SemanticModel { get; set; }
        public Solution Solution { get; set; }
        public Location MacroLocation { get; set; }
    }
}