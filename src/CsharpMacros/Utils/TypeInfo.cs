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
            Namespace = String.Join(".", nameParts.TakeWhile((_, i) => i < nameParts.Length - 1));
        }

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

        public string GetMemberType(ITypeSymbol memberType)
        {
            if (memberType.TypeKind == TypeKind.TypeParameter)
            {
                for (int i = 0; i < Symbol.Arity; i++)
                {
                    if (Symbol.TypeParameters[i].Name == memberType.Name)
                    {
                        return GenericParameterValues[i];
                    }
                }

                return "unknown";
            }

            return memberType.ToString();
        }

    }
}