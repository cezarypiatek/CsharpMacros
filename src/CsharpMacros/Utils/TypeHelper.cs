using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypeInfo = CsharpMacros.Utils.TypeInfo;

namespace CsharpMacros.Macros
{
    class TypeHelper
    {
        public static INamedTypeSymbol FindMatchingSymbol(ICsharpMacroContext context, TypeInfo typeInfo)
        {
            var candidates = context.SemanticModel.Compilation.GetSymbolsWithName(typeInfo.Name)?
                    .OfType<INamedTypeSymbol>()
                    .Where(x => x.ContainingNamespace.ToString().EndsWith(typeInfo.Namespace))
                ;

            if (typeInfo.IsGeneric == false)
            {
                return candidates?.FirstOrDefault();
            }

            return candidates?.FirstOrDefault(x => x.Arity == typeInfo.GenericParameterValues.Length);
        }

        private static TypeInfo GetTypeInfo(string typeName)
        {
            if (typeName.Contains("<"))
            {
                var fullTypeName = typeName.Substring(0, typeName.IndexOf("<", StringComparison.OrdinalIgnoreCase))
                    .Trim();
                var nameParts = fullTypeName.Split('.').ToArray();
                return new TypeInfo(nameParts)
                {
                    IsGeneric = true,
                    GenericParameterValues = GetGenericParameterValues(typeName)
                };
            }

            else
            {
                var nameParts = typeName.Trim().Split('.').ToArray();
                return new TypeInfo(nameParts);
            }
        }

        private static string[] GetGenericParameterValues(string typeName)
        {
            return typeName.Substring(typeName.IndexOf("<", StringComparison.OrdinalIgnoreCase) + 1)
                .TrimEnd()
                .TrimEnd('>')
                .Split(',').Select(x => x.Trim()).ToArray();
        }

        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
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

        public static TypeInfo GetTypeInfo(string typeName, ICsharpMacroContext context)
        {
            var typeInfo = GetTypeInfo(typeName);
            typeInfo.Symbol = FindMatchingSymbol(context, typeInfo);
            return typeInfo;
        }
    }
}