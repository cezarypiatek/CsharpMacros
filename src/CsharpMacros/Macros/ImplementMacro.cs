using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace CsharpMacros.Macros
{
    class ImplementMacro:ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo = TypeHelper.GetTypeInfo(param, context);
            var implementations = SymbolFinder.FindImplementationsAsync(typeInfo.Symbol, context.Solution).GetAwaiter().GetResult();
            foreach (var implementation in implementations.OrderBy(x=>x.Name).OfType<INamedTypeSymbol>())
            {
                var allInterfaces = TypeHelper.GetBaseTypesAndThis(implementation).SelectMany(x => x.Interfaces);

                yield return new Dictionary<string, string>()
                {
                    ["name"] = implementation.GetFullGenericName(),
                    ["interface"] = TypeHelper.FindMatchingSymbol(allInterfaces, typeInfo)?.GetFullGenericName() ?? typeInfo.Name
                };
            }
        }
    }
}
