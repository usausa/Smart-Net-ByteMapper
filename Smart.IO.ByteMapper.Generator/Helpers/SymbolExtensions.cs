namespace Smart.IO.ByteMapper.Generator.Helpers;

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

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

    public static List<IPropertySymbol> GetPropertiesWithBase(this ITypeSymbol type)
    {
        var types = new List<ITypeSymbol>();
        for (var current = type; (current is not null) && (current.SpecialType != SpecialType.System_Object); current = current.BaseType)
        {
            types.Add(current);
        }

        var hidden = new HashSet<string>(StringComparer.Ordinal);
        var properties = new List<IPropertySymbol>[types.Count];
        for (var i = 0; i < types.Count; i++)
        {
            properties[i] = [];
            var declared = new List<string>();
            foreach (var member in types[i].GetMembers())
            {
                if (hidden.Contains(member.Name))
                {
                    continue;
                }
                if (member is IPropertySymbol { IsIndexer: false } property)
                {
                    properties[i].Add(property);
                }
                if (member.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal)
                {
                    declared.Add(member.Name);
                }
            }
            hidden.UnionWith(declared);
        }

        var result = new List<IPropertySymbol>();
        for (var i = types.Count - 1; i >= 0; i--)
        {
            result.AddRange(properties[i]);
        }
        return result;
    }

    public static IPropertySymbol? FindPropertyWithBase(this ITypeSymbol type, string name) =>
        type.GetPropertiesWithBase().FirstOrDefault(x => x.Name == name);

    // Converts a TypedConstant to a C# source-code literal expression.
    // TypedConstant を C# ソースコードのリテラル式文字列に変換する。
    public static string ToLiteralExpression(this TypedConstant constant)
    {
        // Byte values are written as hex, and arrays as byte[], as the byte layout domain expects
        // バイト値は16進、配列は byte[] として出力する（バイトレイアウトの用途に合わせる）
        if (constant.Kind == TypedConstantKind.Primitive)
        {
            if (constant.Value is byte b)
            {
                return $"(byte)0x{b:X2}";
            }

            if (constant.Value is sbyte sb)
            {
                return $"(sbyte){sb}";
            }
        }

        if ((constant.Kind == TypedConstantKind.Array) && !constant.Values.IsDefault)
        {
            var elements = String.Join(", ", constant.Values.Select(static x => x.ToLiteralExpression()));
            return $"new byte[] {{ {elements} }}";
        }

        return constant.ToCSharpExpression() ?? "null";
    }
}
