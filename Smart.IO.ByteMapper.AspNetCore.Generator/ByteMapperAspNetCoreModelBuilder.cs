namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using Microsoft.CodeAnalysis;

using Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// Parse/transform stage: scans a [ByteMapperEndpoint] class and produces one EndpointModel per
// (entity, profile) reader+writer pair. Pure of source emission.
internal static class ByteMapperAspNetCoreModelBuilder
{
    internal const string ByteMapperEndpointAttributeName = "Smart.IO.ByteMapper.AspNetCore.ByteMapperEndpointAttribute";
    private const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    private const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";
    private const string MapProfileAttributeName = "Smart.IO.ByteMapper.MapProfileAttribute";

    public static EquatableArray<EndpointModel> ParseEndpoints(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            return EquatableArray<EndpointModel>.Empty;
        }

        var endpointAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteMapperEndpointAttributeName);
        if (endpointAttr is null)
        {
            return EquatableArray<EndpointModel>.Empty;
        }

        var generateArray = true;
        foreach (var na in endpointAttr.NamedArguments)
        {
            if (na.Key == "GenerateArrayBinding" && na.Value.Value is bool b)
            {
                generateArray = b;
            }
        }

        // Collect all [ByteReader] and [ByteWriter] methods, keyed by (entity FQN, profile FQN or "default").
        // The key ties a reader to its matching writer of the same entity and profile, so multiple
        // entities can coexist in one [ByteMapperEndpoint] class without cross-pairing.
        var readers = new Dictionary<(string Entity, string Profile), (string Name, ITypeSymbol Entity, ITypeSymbol? Profile)>();
        var writers = new Dictionary<(string Entity, string Profile), (string Name, ITypeSymbol Entity, ITypeSymbol? Profile)>();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IMethodSymbol { IsStatic: true } method)
            {
                continue;
            }

            var attrs = method.GetAttributes();

            var readerAttr = attrs.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteReaderAttributeName);
            if (readerAttr is not null)
            {
                ITypeSymbol? profileType = null;
                foreach (var na in readerAttr.NamedArguments)
                {
                    if (na.Key == "Profile" && na.Value.Value is ITypeSymbol pt)
                    {
                        profileType = pt;
                    }
                }

                ITypeSymbol? entityType = null;
                if (!method.ReturnsVoid && method.Parameters.Length == 1)
                {
                    entityType = method.ReturnType;
                }
                else if (method.Parameters.Length == 2)
                {
                    entityType = method.Parameters[1].Type;
                }

                if (entityType is not null)
                {
                    var key = (entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), profileType?.ToDisplayString() ?? "default");
                    if (!readers.ContainsKey(key))
                    {
                        readers[key] = (method.Name, entityType, profileType);
                    }
                }
            }

            var writerAttr = attrs.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteWriterAttributeName);
            if (writerAttr is not null)
            {
                ITypeSymbol? profileType = null;
                foreach (var na in writerAttr.NamedArguments)
                {
                    if (na.Key == "Profile" && na.Value.Value is ITypeSymbol pt)
                    {
                        profileType = pt;
                    }
                }

                ITypeSymbol? entityType = null;
                if (method.ReturnsVoid && method.Parameters.Length == 2)
                {
                    // void Write(Span<byte> destination, T source) — entity is the second parameter.
                    entityType = method.Parameters[1].Type;
                }
                else if (!method.ReturnsVoid
                    && method.Parameters.Length == 1
                    && method.ReturnType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte })
                {
                    // byte[] Write(T source) — entity is the only parameter.
                    entityType = method.Parameters[0].Type;
                }

                if (entityType is not null)
                {
                    var key = (entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), profileType?.ToDisplayString() ?? "default");
                    if (!writers.ContainsKey(key))
                    {
                        writers[key] = (method.Name, entityType, profileType);
                    }
                }
            }
        }

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();
        var rootNs = DetermineRootNamespace(classSymbol);

        // Pair readers with the writer of the same (entity, profile) key. / 同一 (entity, profile) キーの reader/writer をペアリングする
        var pairs = new List<(string ReaderName, string WriterName, ITypeSymbol Entity, ITypeSymbol? Profile, int Size)>();
        foreach (var readerKvp in readers)
        {
            if (!writers.TryGetValue(readerKvp.Key, out var writer))
            {
                continue;
            }

            var (readerMethodName, entityType, profileType) = readerKvp.Value;

            // Size resolution mirrors the core generator: the layout source is the profile type when a
            // profile is specified, and a profile layout is declared by [MapProfile] (a legacy
            // property-mirror profile may still use [Map]). Fall back to the entity's [Map] only when
            // the profile type carries neither.
            // サイズ解決はコアジェネレーターと同じ: プロファイル指定時はプロファイル型がレイアウトソースで、
            // プロファイルレイアウトは [MapProfile] で宣言される（旧式のプロパティミラー型プロファイルは
            // [Map] の場合もある）。どちらも無い場合のみエンティティの [Map] にフォールバックする。
            var sizeSourceType = profileType ?? entityType;
            var mapAttr = sizeSourceType.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() is MapProfileAttributeName or MapAttributeName);
            if (mapAttr is null && profileType is not null)
            {
                mapAttr = entityType.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
            }
            var size = mapAttr is not null && mapAttr.ConstructorArguments.Length > 0
                ? (int)(mapAttr.ConstructorArguments[0].Value ?? 0)
                : -1;
            if (size <= 0)
            {
                continue;
            }

            pairs.Add((readerMethodName, writer.Name, entityType, profileType, size));
        }

        // When the class declares mappers for more than one entity, disambiguate factory names with the
        // entity short name so they do not collide. Single-entity classes keep the plain names.
        // クラスが複数エンティティの mapper を持つ場合のみ、衝突回避のためファクトリ名にエンティティ名を付与する。
        var multipleEntities = pairs
            .Select(p => p.Entity.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Distinct()
            .Count() > 1;

        var results = new List<EndpointModel>();
        foreach (var (readerName, writerName, entityType, profileType, size) in pairs)
        {
            var profileFqn = profileType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var entitySuffix = multipleEntities ? $"_{entityType.Name}" : String.Empty;
            var profileSuffix = profileType is not null ? $"_{profileType.Name}" : String.Empty;
            var nameSuffix = $"{entitySuffix}{profileSuffix}";

            results.Add(new EndpointModel(
                ns,
                classSymbol.Name,
                entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                readerName,
                writerName,
                size,
                profileFqn,
                generateArray,
                rootNs,
                nameSuffix));
        }

        return results.Count == 0
            ? EquatableArray<EndpointModel>.Empty
            : new EquatableArray<EndpointModel>([.. results]);
    }

    private static string DetermineRootNamespace(INamedTypeSymbol symbol)
    {
        var ns = symbol.ContainingNamespace;
        if (ns.IsGlobalNamespace)
        {
            return string.Empty;
        }

        var root = ns;
        while (!root.ContainingNamespace.IsGlobalNamespace)
        {
            root = root.ContainingNamespace;
        }

        return root.Name;
    }
}
