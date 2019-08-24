using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Macros
{
    class MethodsMacro: ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo = TypeHelper.GetTypeInfo(param, context);
            foreach (var member in typeInfo.SelectAllMembers<IMethodSymbol>().Where(x=>x.MethodKind == MethodKind.Ordinary))
            {
                var attributes = new Dictionary<string, string>()
                {
                    ["name"] = member.Name,
                    ["returnType"] = typeInfo.GetMemberType(member.ReturnType)
                };

                int parameterIndex = 1;
                foreach (var parameter in member.Parameters)
                {
                    attributes.Add($"paramName{parameterIndex}", parameter.Name);
                    attributes.Add($"paramType{parameterIndex}", typeInfo.GetMemberType(parameter.Type));
                    parameterIndex++;
                }
                yield return attributes;
            }
        }
    }
}
