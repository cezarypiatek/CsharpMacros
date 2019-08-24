using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Macros
{
    class PropertiesMacro: ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo= TypeHelper.GetTypeInfo(param, context);
            foreach (var member in typeInfo.SelectAllMembers<IPropertySymbol>())
            {
                yield return new Dictionary<string, string>()
                {
                    ["name"] = member.Name,
                    ["type"] = typeInfo.GetMemberType(member.Type)
                };
            }
        }
    }
}