namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using Microsoft.CodeAnalysis;

using Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// Parse/transform stage: scans a [ByteMapperEndpoint] class and produces one EndPointModel per
// (entity, profile) reader+writer pair. Pure of source emission.
internal static class ByteMapperAspNetCoreModelBuilder
{
    internal const string ByteMapperEndpointAttributeName = "Smart.IO.ByteMapper.AspNetCore.ByteMapperEndpointAttribute";
    private const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    private const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";
    private const string MapProfileAttributeName = "Smart.IO.ByteMapper.MapProfileAttribute";
    private const string SetsRequiredMembersAttributeName = "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute";

    public static Result<EndPointCollection> ParseEndPoints(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            return Results.Success(EndPointCollection.Empty);
        }

        var endPointAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteMapperEndpointAttributeName);
        if (endPointAttr is null)
        {
            return Results.Success(EndPointCollection.Empty);
        }

        if (!IsExtendable(classSymbol))
        {
            return Results.Error<EndPointCollection>(new DiagnosticInfo(Diagnostics.InvalidEndpointClass, classSymbol.Locations.FirstOrDefault(), classSymbol.ToDisplayString()));
        }

        var generateArray = true;
        foreach (var na in endPointAttr.NamedArguments)
        {
            if ((na.Key == "GenerateArrayBinding") && (na.Value.Value is bool b))
            {
                generateArray = b;
            }
        }

        var diagnostics = new List<DiagnosticInfo>();

        // Collect all [ByteReader] and [ByteWriter] methods, keyed by (entity FQN, profile FQN or "default").
        // The key ties a reader to its matching writer of the same entity and profile, so multiple
        // entities can coexist in one [ByteMapperEndpoint] class without cross-pairing.
        var readers = new Dictionary<(string Entity, string Profile), (string Name, ITypeSymbol Entity, ITypeSymbol? Profile, Location? Location)>();
        var inPlaceReaders = new HashSet<(string Entity, string Profile)>();
        var writers = new Dictionary<(string Entity, string Profile), (string Name, ITypeSymbol Entity, ITypeSymbol? Profile, bool ReturnsArray, Location? Location)>();

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
                    if ((na.Key == "Profile") && (na.Value.Value is ITypeSymbol pt))
                    {
                        profileType = pt;
                    }
                }

                ITypeSymbol? entityType = null;
                var inPlace = false;
                if (!method.ReturnsVoid && (method.Parameters.Length == 1))
                {
                    entityType = method.ReturnType;
                }
                else if (method.Parameters.Length == 2)
                {
                    entityType = method.Parameters[1].Type;
                    inPlace = method.ReturnsVoid && (method.Parameters[1].RefKind == RefKind.None);
                }

                if (entityType is not null)
                {
                    var key = (entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), profileType?.ToDisplayString() ?? "default");
                    if (!readers.ContainsKey(key) || (inPlace && !inPlaceReaders.Contains(key)))
                    {
                        readers[key] = (method.Name, entityType, profileType, method.Locations.FirstOrDefault());
                    }
                    if (inPlace)
                    {
                        inPlaceReaders.Add(key);
                    }
                }
            }

            var writerAttr = attrs.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteWriterAttributeName);
            if (writerAttr is not null)
            {
                ITypeSymbol? profileType = null;
                foreach (var na in writerAttr.NamedArguments)
                {
                    if ((na.Key == "Profile") && (na.Value.Value is ITypeSymbol pt))
                    {
                        profileType = pt;
                    }
                }

                ITypeSymbol? entityType = null;
                var returnsArray = false;
                if (method.ReturnsVoid && (method.Parameters.Length == 2))
                {
                    // void Write(Span<byte> destination, T source) — entity is the second parameter.
                    entityType = method.Parameters[1].Type;
                }
                else if (!method.ReturnsVoid
                    && (method.Parameters.Length == 1)
                    && (method.ReturnType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte }))
                {
                    // byte[] Write(T source) — entity is the only parameter.
                    entityType = method.Parameters[0].Type;
                    returnsArray = true;
                }

                if (entityType is not null)
                {
                    var key = (entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), profileType?.ToDisplayString() ?? "default");
                    if (!writers.ContainsKey(key))
                    {
                        writers[key] = (method.Name, entityType, profileType, returnsArray, method.Locations.FirstOrDefault());
                    }
                }
            }
        }

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();
        var rootNs = DetermineRootNamespace(classSymbol);

        // Pair readers with the writer of the same (entity, profile) key. / 同一 (entity, profile) キーの reader/writer をペアリングする
        var pairs = new List<(string ReaderName, string WriterName, bool WriterReturnsArray, ITypeSymbol Entity, ITypeSymbol? Profile, int Size)>();
        foreach (var writerKvp in writers)
        {
            if (!inPlaceReaders.Contains(writerKvp.Key))
            {
                diagnostics.Add(new DiagnosticInfo(
                    Diagnostics.WriterWithoutReader,
                    writerKvp.Value.Location,
                    writerKvp.Value.Name,
                    writerKvp.Value.Entity.ToDisplayString()));
            }
        }

        foreach (var readerKvp in readers)
        {
            if (!writers.TryGetValue(readerKvp.Key, out var writer))
            {
                // A reader with no matching writer cannot form a binding; report it instead of
                // silently dropping the endPoint.
                diagnostics.Add(new DiagnosticInfo(
                    Diagnostics.ReaderWithoutWriter,
                    readerKvp.Value.Location,
                    readerKvp.Value.Name,
                    readerKvp.Value.Entity.ToDisplayString()));
                continue;
            }

            if (!inPlaceReaders.Contains(readerKvp.Key))
            {
                continue;
            }

            var (readerMethodName, entityType, profileType, readerLocation) = readerKvp.Value;

            if (!IsCreatable(context.SemanticModel, context.TargetNode.SpanStart, entityType))
            {
                diagnostics.Add(new DiagnosticInfo(Diagnostics.EntityNotCreatable, readerLocation, readerMethodName, entityType.ToDisplayString()));
                continue;
            }

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
            if ((mapAttr is null) && (profileType is not null))
            {
                mapAttr = entityType.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
            }
            var size = (mapAttr is not null) && mapAttr.TryGetConstructorArgument<int>(0, out var mapSize) ? mapSize : -1;
            if (size <= 0)
            {
                // Without a positive [Map]/[MapProfile] size the binding cannot be emitted.
                diagnostics.Add(new DiagnosticInfo(
                    Diagnostics.UnknownEntitySize,
                    classSymbol.Locations.FirstOrDefault(),
                    entityType.ToDisplayString()));
                continue;
            }

            pairs.Add((readerMethodName, writer.Name, writer.ReturnsArray, entityType, profileType, size));
        }

        // When the class declares mappers for more than one entity, disambiguate factory names with the
        // entity short name so they do not collide. Single-entity classes keep the plain names.
        // クラスが複数エンティティの mapper を持つ場合のみ、衝突回避のためファクトリ名にエンティティ名を付与する。
        var multipleEntities = pairs
            .Select(p => p.Entity.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Distinct()
            .Count() > 1;
        var qualifyEntities = HasCollidingNames(pairs.Select(static p => p.Entity));
        var qualifyProfiles = HasCollidingNames(pairs.Select(static p => p.Profile).OfType<ITypeSymbol>());

        var containingTypes = new EquatableArray<string>(classSymbol.GetContainingTypes()
            .Append(classSymbol)
            .Select(static x => x.GetPartialDeclaration())
            .ToArray());
        var classFullName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var results = new List<EndPointModel>();
        foreach (var (readerName, writerName, writerReturnsArray, entityType, profileType, size) in pairs)
        {
            var profileFqn = profileType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var entitySuffix = multipleEntities ? "_" + SuffixName(entityType, qualifyEntities) : string.Empty;
            var profileSuffix = profileType is not null ? "_" + SuffixName(profileType, qualifyProfiles) : string.Empty;
            var nameSuffix = $"{entitySuffix}{profileSuffix}";

            results.Add(new EndPointModel(
                ns,
                classSymbol.Name,
                containingTypes,
                classFullName,
                HintNameBuilder.BuildFromTypeWithExtension(classSymbol, ".AspNetCore.g.cs", nameSuffix.TrimStart('_')),
                readerName,
                writerName,
                writerReturnsArray,
                entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                IsPublic(entityType),
                profileFqn,
                size,
                generateArray,
                rootNs,
                nameSuffix));
        }

        var collection = new EndPointCollection(new EquatableArray<EndPointModel>(results));
        return diagnostics.Count == 0
            ? Results.Success(collection)
            : new Result<EndPointCollection>(collection, new EquatableArray<DiagnosticInfo>(diagnostics));
    }

    private static bool IsExtendable(INamedTypeSymbol classSymbol)
    {
        if (classSymbol.IsFileLocal)
        {
            return false;
        }

        for (var current = classSymbol; current is not null; current = current.ContainingType)
        {
            if (current.IsGenericType || !IsPartial(current) ||
                (current.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected or Accessibility.ProtectedAndInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPartial(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if ((reference.GetSyntax() is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax declaration) &&
                declaration.Modifiers.Any(static x => x.Text == "partial"))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsCreatable(SemanticModel semanticModel, int position, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol named)
        {
            return false;
        }

        if (named.IsValueType)
        {
            return !HasRequiredMembers(named);
        }

        return (named.TypeKind == TypeKind.Class) && !named.IsAbstract &&
               named.InstanceConstructors.Any(c => (c.Parameters.Length == 0) && semanticModel.IsAccessible(position, c) &&
                   !(c.IsObsolete(out var isError) && isError) &&
                   (!HasRequiredMembers(named) || c.HasAttribute(SetsRequiredMembersAttributeName)));
    }

    private static bool HasRequiredMembers(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.GetMembers().Any(static x => x is IPropertySymbol { IsRequired: true } or IFieldSymbol { IsRequired: true }))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasCollidingNames(IEnumerable<ITypeSymbol> types) =>
        types
            .Distinct<ITypeSymbol>(SymbolEqualityComparer.Default)
            .GroupBy(static x => x.Name, StringComparer.Ordinal)
            .Any(static x => x.Count() > 1);

    private static string SuffixName(ITypeSymbol type, bool qualify)
    {
        if (!qualify)
        {
            return type.Name;
        }

        var name = type.ToDisplayString();
        var builder = new System.Text.StringBuilder(name.Length);
        foreach (var c in name)
        {
            builder.Append(Char.IsLetterOrDigit(c) ? c : '_');
        }
        return builder.ToString();
    }

    private static bool IsPublic(ITypeSymbol type)
    {
        for (var current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }
        }
        return true;
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
