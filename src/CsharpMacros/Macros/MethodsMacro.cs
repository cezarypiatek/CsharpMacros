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
                var (returnTypeShorty, returnTypeLong) = typeInfo.GetMemberType(member.ReturnType);
                var attributes = new Dictionary<string, string>()
                {
                    ["name"] = member.Name,
                    ["returnType"] = returnTypeShorty,
                    ["returnTypeLong"] = returnTypeLong
                };

                int parameterIndex = 1;
                foreach (var parameter in member.Parameters)
                {
                    var (paramTypeShort, paramTypeLong) = typeInfo.GetMemberType(parameter.Type);
                    attributes.Add($"paramName{parameterIndex}", parameter.Name);
                    attributes.Add($"paramType{parameterIndex}", paramTypeShort );
                    attributes.Add($"paramTypeLong{parameterIndex}", paramTypeLong );
                    parameterIndex++;
                }
                yield return attributes;
            }
        }
    }
}
