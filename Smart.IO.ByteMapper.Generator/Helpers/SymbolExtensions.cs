namespace Smart.IO.ByteMapper.Generator.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal static class SymbolExtensions
{
    public static string GetClassName(this INamedTypeSymbol type)
    {
        var name = type.Name;
        if (type.TypeParameters.Length > 0)
        {
            name += $"<{string.Join(", ", type.TypeParameters.Select(p => p.Name))}>";
        }
        return name;
    }

    public static bool IsNullableSymbol(this ITypeSymbol type)
    {
        if (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return true;
        }

        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return true;
        }
        return false;
    }

    // Gets the non-nullable underlying type of Nullable<T> or nullable reference type.
    public static ITypeSymbol GetNonNullableType(this ITypeSymbol type)
    {
        if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return ((INamedTypeSymbol)type).TypeArguments[0];
        }

        if (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }
        return type;
    }

    // Walks up the base class chain of attributeClass and returns the first constructed instance
    // of the open generic type ByteMapperConverterAttribute<>.
    // Returns null if the attribute does not derive from that open generic.
    public static INamedTypeSymbol? FindConverterAttributeBase(this INamedTypeSymbol attributeClass, INamedTypeSymbol converterAttributeOpenGeneric)
    {
        var current = attributeClass.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, converterAttributeOpenGeneric))
            {
                return current;
            }
            current = current.BaseType;
        }
        return null;
    }

    // Returns true when type inherits from baseType.
    public static bool InheritsFrom(this ITypeSymbol type, INamedTypeSymbol baseType)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, baseType.OriginalDefinition))
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

    // Converts a TypedConstant to a C# source-code literal expression.
    public static string ToLiteralExpression(this TypedConstant constant)
    {
        if (constant.Kind == TypedConstantKind.Primitive)
        {
            return constant.Value switch
            {
                null => "null",
                bool b => b ? "true" : "false",
                string s => $"\"{s.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"",
                byte b => $"(byte)0x{b:X2}",
                sbyte sb => $"(sbyte){sb}",
                char c => $"'{c}'",
                _ => constant.Value!.ToString() ?? "null"
            };
        }
        if (constant.Kind == TypedConstantKind.Enum && constant.Type != null)
        {
            var fqn = constant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $"{fqn}.{GetEnumMemberName(constant)}";
        }
        if (constant.Kind == TypedConstantKind.Array && constant.Values != null)
        {
            var elements = string.Join(", ", constant.Values.Select(v => v.ToLiteralExpression()));
            return $"new byte[] {{ {elements} }}";
        }
        return constant.Value?.ToString() ?? "null";
    }

    private static string GetEnumMemberName(TypedConstant constant)
    {
        if ((constant.Type == null) || (constant.Value == null))
        {
            return constant.Value?.ToString() ?? "0";
        }
        var enumType = constant.Type;
        foreach (var member in enumType.GetMembers())
        {
            if (member is IFieldSymbol field && field.HasConstantValue && Equals(field.ConstantValue, constant.Value))
            {
                return field.Name;
            }
        }
        return constant.Value.ToString() ?? "0";
    }
}
