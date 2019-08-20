using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Macros
{
    class PropertiesMacro: ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeName = param;
            var typeInfo = context.SemanticModel.Compilation.GetSymbolsWithName(typeName)?.OfType<INamedTypeSymbol>().FirstOrDefault();
            if (typeInfo != null)
            {
                var members = typeInfo.GetMembers();
                foreach (var member in members.OfType<IPropertySymbol>())
                {
                    yield return new Dictionary<string, string>()
                    {
                        ["name"] = member.Name,
                        ["type"] = member.Type.ToString()
                    };
                }
            }
        }
    }
}