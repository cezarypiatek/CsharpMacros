using System;
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
                    ["returnTypeLong"] = returnTypeLong,
                    ["signature"] = GetSignature(member, context),
                    ["returnOperator"] = GetReturnOperator(returnTypeShorty),
                    ["parameters"] = GetParametersString(member),
                    ["genericParameters"] = GetGenericParametersString(member)
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

        private static string GetParametersString(IMethodSymbol member)
        {
            
            var parametersString = string.Join(", ", member.Parameters.Select(x=>
                {
                    switch (x.RefKind)
                    {
                        case RefKind.None:
                            return $"{x.Name}";
                        case RefKind.Ref:
                            return $"ref {x.Name}";
                        case RefKind.Out:
                            return $"out {x.Name}";
                        case RefKind.In:
                            return $"in {x.Name}";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));
            return $"({parametersString})";
        } 
        private static string GetGenericParametersString(IMethodSymbol member)
        {
            var genericParametersString = string.Join(", ", member.TypeParameters.Select(x=>x.ToDisplayString()));
            return string.IsNullOrWhiteSpace(genericParametersString)? "": $"<{genericParametersString}>";
        }

        private static string GetReturnOperator(string returnTypeShorty)
        {
            return returnTypeShorty == "void"? "": "return";
        }

        private static string GetSignature(IMethodSymbol member, ICsharpMacroContext context)
        {
            string PresentSymbol(ISymbol typeSymbol)
            {
                return typeSymbol.ToMinimalDisplayString(context.SemanticModel, context.MacroLocation.SourceSpan.Start);
            }

            var parameters = string.Join(", ", member.Parameters.Select(x =>$"{PresentSymbol(x)}".Trim()));
            

            if (member.IsGenericMethod)
            {
                var genericParameters = GetGenericParametersString(member); 
                return $"{PresentSymbol(member.ReturnType)} {member.Name}{genericParameters}({parameters})";
            }
            return $"{PresentSymbol(member.ReturnType)} {member.Name}({parameters})";
        }
    }
}
