namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SourceGenerateHelper;

// Incremental generator that emits ByteMapperBinding factory methods,
// assembly-level bootstrap, and AddByteMapperFormatters extension for
// classes annotated with [ByteMapperEndpoint].
[Generator]
public sealed class ByteMapperAspNetCoreGenerator : IIncrementalGenerator
{
    private const string ByteMapperEndpointAttributeName = "Smart.IO.ByteMapper.AspNetCore.ByteMapperEndpointAttribute";
    private const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    private const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var endpoints = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperEndpointAttributeName,
                static (s, _) => s is ClassDeclarationSyntax,
                static (ctx, _) => ParseEndpoints(ctx))
            .Where(static arr => arr.Count > 0)
            .SelectMany(static (arr, _) => arr)
            .Collect();

        context.RegisterImplementationSourceOutput(
            endpoints,
            static (spc, items) => Execute(spc, items));
    }

    // -------------------------------------------------------
    // Parse
    // -------------------------------------------------------

    private static EquatableArray<EndpointModel> ParseEndpoints(GeneratorAttributeSyntaxContext context)
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

    // -------------------------------------------------------
    // Execute
    // -------------------------------------------------------

    private static void Execute(
        SourceProductionContext spc,
        ImmutableArray<EndpointModel> endpoints)
    {
        if (endpoints.IsDefaultOrEmpty)
        {
            return;
        }

        var builder = new SourceBuilder();

        foreach (var ep in endpoints)
        {
            builder.Clear();
            BuildBindingSource(builder, ep);
            var filename = MakeFilename(ep.Namespace, ep.ClassName, ep.ProfileTypeFqn) + ".AspNetCore.g.cs";
            spc.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        builder.Clear();
        BuildBootstrapSource(builder, endpoints);
        spc.AddSource("__ByteMapperAspNetCoreBootstrap.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    // -------------------------------------------------------
    // Source builders
    // -------------------------------------------------------

    private static void BuildBindingSource(SourceBuilder builder, EndpointModel ep)
    {
        builder.AutoGenerated();
        builder.EnableNullable();
        builder.NewLine();

        if (!String.IsNullOrEmpty(ep.Namespace))
        {
            builder.Namespace(ep.Namespace);
            builder.NewLine();
        }

        builder.Indent().Append("partial class ").Append(ep.ClassName).NewLine();
        builder.BeginScope();

        // Single binding factory
        builder.Indent()
            .Append("public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding<").Append(ep.EntityTypeFqn).Append("> ").Append(ep.FactoryMethodName).Append("()")
            .NewLine();
        builder.Indent().Append("    => new(").NewLine();
        builder.Indent().Append("        size: ").Append(ep.Size.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(",").NewLine();
        builder.Indent().Append("        read:    static (s, t) => ").Append(ep.ReaderMethodName).Append("(s, t),").NewLine();
        builder.Indent().Append("        write:   static (s, d) => ").Append(ep.WriterMethodName).Append("(d, s),").NewLine();
        builder.Indent().Append("        factory: static () => new ").Append(ep.EntityTypeFqn).Append("());").NewLine();

        if (ep.GenerateArrayBinding)
        {
            builder.NewLine();

            // Array binding factory
            builder.Indent()
                .Append("public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperArrayBinding<").Append(ep.EntityTypeFqn).Append("> ").Append(ep.ArrayFactoryMethodName).Append("()")
                .NewLine();
            builder.Indent().Append("    => new(").NewLine();
            builder.Indent().Append("        elementSize:  ").Append(ep.Size.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(",").NewLine();
            builder.Indent().Append("        readElement:  static (s, t) => ").Append(ep.ReaderMethodName).Append("(s, t),").NewLine();
            builder.Indent().Append("        writeElement: static (s, d) => ").Append(ep.WriterMethodName).Append("(d, s),").NewLine();
            builder.Indent().Append("        factory:      static () => new ").Append(ep.EntityTypeFqn).Append("());").NewLine();
        }

        builder.EndScope();
    }

    private static void BuildBootstrapSource(SourceBuilder builder, ImmutableArray<EndpointModel> endpoints)
    {
        var rootNs = string.Empty;
        foreach (var e in endpoints)
        {
            if (!String.IsNullOrEmpty(e.RootNamespace))
            {
                rootNs = e.RootNamespace;
                break;
            }
        }
        if (String.IsNullOrEmpty(rootNs))
        {
            rootNs = "ByteMapperGenerated";
        }

        builder.AutoGenerated();
        builder.EnableNullable();
        builder.NewLine();
        builder.Namespace(rootNs);
        builder.NewLine();
        builder.Indent().Append("using System.Collections.Generic;").NewLine();
        builder.NewLine();

        // Bootstrap class
        builder.Indent().Append("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]").NewLine();
        builder.Indent().Append("internal static class __ByteMapperAspNetCoreBootstrap").NewLine();
        builder.BeginScope();

        builder.Indent().Append("public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry Build()").NewLine();
        builder.BeginScope();

        builder.Indent().Append("var single = new Dictionary<(global::System.Type, global::System.Type?), global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding>").NewLine();
        builder.Indent().Append("{").NewLine();
        foreach (var ep in endpoints)
        {
            var qualifiedClass = String.IsNullOrEmpty(ep.Namespace)
                ? $"global::{ep.ClassName}"
                : $"global::{ep.Namespace}.{ep.ClassName}";
            var profileLiteral = ep.ProfileTypeFqn is null ? "null" : $"typeof({ep.ProfileTypeFqn})";
            builder.Indent().Append("    { (typeof(").Append(ep.EntityTypeFqn).Append("), ").Append(profileLiteral).Append("), ").Append(qualifiedClass).Append(".").Append(ep.FactoryMethodName).Append("() },").NewLine();
        }
        builder.Indent().Append("};").NewLine();
        builder.NewLine();

        builder.Indent().Append("var array = new Dictionary<(global::System.Type, global::System.Type?), object>").NewLine();
        builder.Indent().Append("{").NewLine();
        foreach (var ep in endpoints)
        {
            if (!ep.GenerateArrayBinding)
            {
                continue;
            }

            var qualifiedClass = String.IsNullOrEmpty(ep.Namespace)
                ? $"global::{ep.ClassName}"
                : $"global::{ep.Namespace}.{ep.ClassName}";
            var profileLiteral = ep.ProfileTypeFqn is null ? "null" : $"typeof({ep.ProfileTypeFqn})";
            builder.Indent().Append("    { (typeof(").Append(ep.EntityTypeFqn).Append("), ").Append(profileLiteral).Append("), ").Append(qualifiedClass).Append(".").Append(ep.ArrayFactoryMethodName).Append("() },").NewLine();
        }
        builder.Indent().Append("};").NewLine();
        builder.NewLine();

        builder.Indent().Append("return new global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry(single, array);").NewLine();
        builder.EndScope();

        builder.EndScope();
        builder.NewLine();

        // AddByteMapperFormatters extension
        builder.Indent().Append("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]").NewLine();
        builder.Indent().Append("internal static class __ByteMapperServiceCollectionExtensions").NewLine();
        builder.BeginScope();

        builder.Indent().Append("// Registers ByteMapper registry, formatters, and options. Generated by source generator.").NewLine();
        builder.Indent().Append("public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddByteMapperFormatters(").NewLine();
        builder.Indent().Append("    this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,").NewLine();
        builder.Indent().Append("    global::System.Action<global::Smart.IO.ByteMapper.AspNetCore.ByteMapperFormatterOptions>? configure = null)").NewLine();
        builder.Indent().Append("    => global::Smart.IO.ByteMapper.AspNetCore.ByteMapperServiceCollectionExtensions.AddByteMapperFormatters(").NewLine();
        builder.Indent().Append("        services,").NewLine();
        builder.Indent().Append("        __ByteMapperAspNetCoreBootstrap.Build(),").NewLine();
        builder.Indent().Append("        configure);").NewLine();

        builder.EndScope();
    }

    private static string MakeFilename(string ns, string className, string? profileFqn)
    {
        var buffer = new StringBuilder();
        if (!String.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }
        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        if (!String.IsNullOrEmpty(profileFqn))
        {
            var lastDot = profileFqn!.LastIndexOf('.');
            var shortName = lastDot >= 0 ? profileFqn.Substring(lastDot + 1) : profileFqn;
            buffer.Append('_').Append(shortName);
        }
        return buffer.ToString();
    }

    // -------------------------------------------------------
    // Model
    // -------------------------------------------------------

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via ImmutableArray.CreateBuilder<EndpointModel>.Add.")]
    private sealed record EndpointModel(
        string Namespace,
        string ClassName,
        string EntityTypeFqn,
        string ReaderMethodName,
        string WriterMethodName,
        int Size,
        string? ProfileTypeFqn,
        bool GenerateArrayBinding,
        string RootNamespace,
        string FactoryMethodName,
        string ArrayFactoryMethodName);
}
