using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Macros
{
    class PropertiesMacro: ICsharpMacro
    {
        public IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context)
        {
            var typeInfo= GetTypeNameWithoutGenericParameters(param);
            var typeSymbol = FindMatchingSymbol(context, typeInfo);
            if (typeSymbol != null)
            {
                var members = GetBaseTypesAndThis(typeSymbol).Reverse().SelectMany(t=> t.GetMembers());
                foreach (var member in members.OfType<IPropertySymbol>())
                {
                    yield return new Dictionary<string, string>()
                    {
                        ["name"] = member.Name,
                        ["type"] = GetMemberType(member, typeInfo, typeSymbol)
                    };
                }
            }
        }

        private static string GetMemberType(IPropertySymbol member, TypeInfo typeInfo, INamedTypeSymbol typeSymbol)
        {
            if (member.Type.TypeKind == TypeKind.TypeParameter)
            {
                for (int i = 0; i < typeSymbol.Arity; i++)
                {
                    if (typeSymbol.TypeParameters[i].Name == member.Type.Name)
                    {
                        return typeInfo.GenericParameterValues[i];
                    }
                }

                return "unknown";
            }

            return member.Type.ToString();
        }

        private static INamedTypeSymbol FindMatchingSymbol(ICsharpMacroContext context, TypeInfo typeInfo)
        {
            var candidates = context.SemanticModel.Compilation.GetSymbolsWithName(typeInfo.Name)?.OfType<INamedTypeSymbol>();

            if (typeInfo.IsGeneric == false)
            {
                return candidates?.FirstOrDefault();
            }

            return candidates?.FirstOrDefault(x=> x.Arity == typeInfo.GenericParameterValues.Length);
        }

        private TypeInfo GetTypeNameWithoutGenericParameters(string typeName)
        {
            if (typeName.Contains("<"))
            {
                return new TypeInfo()
                {
                    Name = typeName.Substring(0, typeName.IndexOf("<", StringComparison.OrdinalIgnoreCase)).Trim(),
                    IsGeneric = true,
                    GenericParameterValues = GetGenericParameterValues(typeName)
                };
            }

            return new TypeInfo()
            {
                Name = typeName.Trim()
            };
        }

        private static string[] GetGenericParameterValues(string typeName)
        {
            return typeName.Substring(typeName.IndexOf("<", StringComparison.OrdinalIgnoreCase) +1)
                .TrimEnd()
                .TrimEnd('>')
                .Split(',').Select(x=>x.Trim()).ToArray();
        }

        class TypeInfo
        {
            public string Name { get; set; }
            public bool IsGeneric { get; set; }
            public string[] GenericParameterValues { get; set; }
        }

        private static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
        {
            foreach (var unwrapped in UnwrapGeneric(type))
            {
                var current = unwrapped;
                while (current != null && IsSystemObject(current) == false)
                {
                    yield return current;
                    current = current.BaseType;
                }
            }
        }

        private static IEnumerable<ITypeSymbol> UnwrapGeneric(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind == TypeKind.TypeParameter && typeSymbol is ITypeParameterSymbol namedType && namedType.Kind != SymbolKind.ErrorType)
            {
                return namedType.ConstraintTypes;
            }
            return new[] { typeSymbol };
        }

        private static bool IsSystemObject(ITypeSymbol current)
        {
            return current.Name == "Object" && current.ContainingNamespace.Name == "System";
        }
    }
}