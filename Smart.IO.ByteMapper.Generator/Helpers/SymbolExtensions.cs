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
        while (current is not null)
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
        while (current is not null)
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
        if (constant.Kind == TypedConstantKind.Enum && constant.Type is not null)
        {
            var fqn = constant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var memberName = FindEnumMemberName(constant);

            // A combined flags value (e.g. NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign)
            // has no single member name; emit a cast expression instead of an invalid "Type.36" literal.
            // フラグ合成値（例: NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign）は単一の
            // メンバー名を持たないため、不正な "Type.36" ではなくキャスト式を出力する。
            return memberName is not null
                ? $"{fqn}.{memberName}"
                : $"({fqn})({constant.Value})";
        }
        if (constant.Kind == TypedConstantKind.Array && constant.Values != null)
        {
            var elements = String.Join(", ", constant.Values.Select(v => v.ToLiteralExpression()));
            return $"new byte[] {{ {elements} }}";
        }
        return constant.Value?.ToString() ?? "null";
    }

    // Returns the enum member name for an exact value match, or null when the value is not a single
    // named member (e.g. a combined flags value).
    // 値が単一メンバーに一致する場合はその名前を、一致しない場合（フラグ合成値など）は null を返す。
    private static string? FindEnumMemberName(TypedConstant constant)
    {
        if ((constant.Type is null) || (constant.Value is null))
        {
            return null;
        }
        var enumType = constant.Type;
        foreach (var member in enumType.GetMembers())
        {
            if (member is IFieldSymbol field && field.HasConstantValue && Equals(field.ConstantValue, constant.Value))
            {
                return field.Name;
            }
        }
        return null;
    }
}
