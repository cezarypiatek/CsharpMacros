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
            var typeSymbol = context.SemanticModel.Compilation.GetSymbolsWithName(typeName)?.OfType<INamedTypeSymbol>().FirstOrDefault();
            if (typeSymbol != null)
            {
                var members = GetBaseTypesAndThis(typeSymbol).Reverse().SelectMany(t=> t.GetMembers());
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