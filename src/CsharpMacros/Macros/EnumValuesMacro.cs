using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Macros
{
    class EnumValuesMacro: ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo = TypeHelper.GetTypeInfo(param, context);
            if (typeInfo.Symbol is { } symbol && symbol.TypeKind == TypeKind.Enum)
            {
                foreach (var member in symbol.GetMembers())
                {
                    if (member.Kind == SymbolKind.Field)
                    {
                       yield return new Dictionary<string, string>()
                       {
                           ["name"] = member.Name
                       };
                    }
                }
            }
        }
    }
}
