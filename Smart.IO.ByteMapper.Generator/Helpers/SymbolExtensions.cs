namespace Smart.IO.ByteMapper.Generator.Helpers;

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

internal static class SymbolExtensions
{
    // Sizes of well-known unmanaged primitive types (used to resolve BinaryConverter<T>.Size at code-gen time).
    // 既知のアンマネージドプリミティブ型のサイズマップ（コード生成時に BinaryConverter<T>.Size を解決するために使用）
    private static readonly Dictionary<string, int> KnownUnmanagedSizes = new(StringComparer.Ordinal)
    {
        ["byte"] = 1,
        ["sbyte"] = 1,
        ["short"] = 2,
        ["ushort"] = 2,
        ["int"] = 4,
        ["uint"] = 4,
        ["long"] = 8,
        ["ulong"] = 8,
        ["float"] = 4,
        ["double"] = 8,
        ["decimal"] = 16,
        ["System.Byte"] = 1,
        ["System.SByte"] = 1,
        ["System.Int16"] = 2,
        ["System.UInt16"] = 2,
        ["System.Int32"] = 4,
        ["System.UInt32"] = 4,
        ["System.Int64"] = 8,
        ["System.UInt64"] = 8,
        ["System.Single"] = 4,
        ["System.Double"] = 8,
        ["System.Decimal"] = 16
    };

    // Tries to get the unmanaged byte size for a well-known primitive type symbol.
    // 既知のプリミティブ型シンボルに対応するアンマネージドバイトサイズの取得を試みる。
    public static bool TryGetUnmanagedSize(this ITypeSymbol typeArg, out int size)
    {
        var typeKey = typeArg.SpecialType != SpecialType.None
            ? typeArg.ToDisplayString()
            : typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", string.Empty);
        return KnownUnmanagedSizes.TryGetValue(typeKey, out size);
    }

    // Walks up the base class chain of attributeClass and returns the first constructed instance
    // of the open generic type ByteMapperPropertyAttribute<>.
    // Returns null if the attribute does not derive from that open generic.
    // attributeClass の基底クラスチェーンを辿り、オープンジェネリック型 ByteMapperPropertyAttribute<> の
    // 最初の構築済みインスタンスを返す。派生していない場合は null を返す。
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
    // type が baseType を継承している場合に true を返す。
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
    // TypedConstant を C# ソースコードのリテラル式文字列に変換する。
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
