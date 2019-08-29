using System;
using System.Collections.Generic;
using System.Linq;
using CsharpMacros.Macros;
using Microsoft.CodeAnalysis;

namespace CsharpMacros.Utils
{
    class TypeInfo
    {
        public TypeInfo(string[] nameParts)
        {
            Name = nameParts.Last();
            Namespace = string.Join(".", nameParts.TakeWhile((_, i) => i < nameParts.Length - 1));
            FullName = string.Join(".", nameParts);
        }

        public string FullName { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }
        public bool IsGeneric { get; set; }
        public string[] GenericParameterValues { get; set; }

        public INamedTypeSymbol Symbol { get; set; }

        public IEnumerable<T> SelectAllMembers<T>()
        {
            if (Symbol == null)
            {
                return Enumerable.Empty<T>();
            }

            return TypeHelper.GetBaseTypesAndThis(Symbol).Reverse().SelectMany(x=>x.GetMembers()).OfType<T>();
        }

        public (string shortName, string longName) GetMemberType(ITypeSymbol memberType)
        {
            if (memberType.TypeKind == TypeKind.TypeParameter)
            {
                for (int i = 0; i < Symbol.Arity; i++)
                {
                    if (Symbol.TypeParameters[i].Name == memberType.Name)
                    {
                        var genericTypeName = GenericParameterValues[i];
                        return (genericTypeName, TryGetTypeLongName(genericTypeName));
                    }
                }

                return ("unknown", "unknown");
            }

            return (memberType.ToString(), memberType.Name);
        }

        private string TryGetTypeLongName(string shortName)
        {
            if (typeLongNameMap.ContainsKey(shortName))
            {
                return typeLongNameMap[shortName];
            }

            return shortName;
        }

        private static Dictionary<string, string> typeLongNameMap = new Dictionary<string, string>()
        {
            ["object"]= "Object",
            ["string"] = "String",
            ["bool"] = "Boolean",
            ["byte"] = "Byte",
            ["char"] = "Char",
            ["decimal"] = "Decimal",
            ["double"] = "Double",
            ["short"] = "Int16",
            ["int"] = "Int32",
            ["long"] = "Int64",
            ["sbyte"] = "SByte",
            ["float"] = "Single",
            ["ushort"] = "UInt16",
            ["uint"] = "UInt32",
            ["ulong"] = "UInt64",
            ["void"] = "Void",
        };

    }
}