using System.Collections.Generic;
using Microsoft.CodeAnalysis.FindSymbols;

namespace CsharpMacros.Macros
{
    class DerivedMacro:ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo = TypeHelper.GetTypeInfo(param, context);
            var derived = SymbolFinder.FindDerivedClassesAsync(typeInfo.Symbol, context.Solution).GetAwaiter().GetResult();
            foreach (var derivedType in derived)
            {
                yield return new Dictionary<string, string>()
                {
                    ["name"] = derivedType.GetFullGenericName(),
                    ["based"] = typeInfo.Symbol.Name
                };
            }
        }
    }
}