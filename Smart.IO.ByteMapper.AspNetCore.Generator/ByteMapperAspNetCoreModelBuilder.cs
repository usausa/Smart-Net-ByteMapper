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

        // Collect all [ByteReader] and [ByteWriter] methods, keyed by profile FQN (or "default").
        // The key ties a reader to its matching writer.
        var readers = new Dictionary<string, (string Name, ITypeSymbol Entity, ITypeSymbol? Profile)>();
        var writers = new Dictionary<string, (string Name, ITypeSymbol Entity, ITypeSymbol? Profile)>();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IMethodSymbol method || !method.IsStatic)
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
                    var key = profileType?.ToDisplayString() ?? "default";
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
                    entityType = method.Parameters[0].Type;
                }
                else if (!method.ReturnsVoid
                    && method.Parameters.Length == 1
                    && method.ReturnType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte })
                {
                    entityType = method.Parameters[0].Type;
                }

                if (entityType is not null)
                {
                    var key = profileType?.ToDisplayString() ?? "default";
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

        var results = new List<EndpointModel>();

        foreach (var readerKvp in readers)
        {
            if (!writers.TryGetValue(readerKvp.Key, out var writer))
            {
                continue;
            }

            var (readerMethodName, entityType, profileType) = readerKvp.Value;

            var sizeSourceType = profileType ?? entityType;
            var mapAttr = sizeSourceType.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
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

            var profileFqn = profileType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var profileSuffix = profileType?.Name ?? String.Empty;
            var factoryName = String.IsNullOrEmpty(profileSuffix)
                ? "CreateByteMapperBinding"
                : $"CreateByteMapperBinding_{profileSuffix}";
            var arrayFactoryName = String.IsNullOrEmpty(profileSuffix)
                ? "CreateByteMapperArrayBinding"
                : $"CreateByteMapperArrayBinding_{profileSuffix}";

            results.Add(new EndpointModel(
                ns,
                classSymbol.Name,
                entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                readerMethodName,
                writer.Name,
                size,
                profileFqn,
                generateArray,
                rootNs,
                factoryName,
                arrayFactoryName));
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
