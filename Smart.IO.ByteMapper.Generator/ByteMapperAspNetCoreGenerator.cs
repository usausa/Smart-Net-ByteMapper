namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SourceGenerateHelper;

/// <summary>
/// Incremental generator that emits ByteMapperBinding factory methods,
/// assembly-level bootstrap, and AddByteMapperFormatters extension for
/// classes annotated with [ByteMapperEndpoint].
/// </summary>
[Generator]
public sealed class ByteMapperAspNetCoreGenerator : IIncrementalGenerator
{
    private const string ByteMapperEndpointAttributeName = "Smart.IO.ByteMapper.AspNetCore.ByteMapperEndpointAttribute";
    private const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    private const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Detect classes annotated with [ByteMapperEndpoint]
        var endpoints = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperEndpointAttributeName,
                static (s, _) => s is ClassDeclarationSyntax,
                static (ctx, _) => ParseEndpoint(ctx))
            .Where(static e => e is not null)
            .Collect();

        context.RegisterImplementationSourceOutput(
            endpoints,
            static (spc, items) => Execute(spc, items!));
    }

    // -------------------------------------------------------
    // Parse
    // -------------------------------------------------------

    private static EndpointModel? ParseEndpoint(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        // Read [ByteMapperEndpoint] arguments
        var endpointAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ByteMapperEndpointAttributeName);
        if (endpointAttr is null) return null;

        string? profileKey = null;
        var generateArray = true;
        foreach (var na in endpointAttr.NamedArguments)
        {
            if (na.Key == "Key" && na.Value.Value is string k) profileKey = k;
            if (na.Key == "GenerateArrayBinding" && na.Value.Value is bool b) generateArray = b;
        }

        // Find [ByteReader] and [ByteWriter] methods in the class
        string? readerMethodName = null;
        string? writerMethodName = null;
        ITypeSymbol? entityType = null;

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IMethodSymbol method || !method.IsStatic) continue;

            var attrs = method.GetAttributes();
            var isReader = attrs.Any(a => a.AttributeClass?.ToDisplayString() == ByteReaderAttributeName);
            var isWriter = attrs.Any(a => a.AttributeClass?.ToDisplayString() == ByteWriterAttributeName);

            if (isReader && readerMethodName is null)
            {
                readerMethodName = method.Name;
                // entity type: second param (in-place) or return type (new-instance)
                if (!method.ReturnsVoid && method.Parameters.Length == 1)
                {
                    entityType = method.ReturnType;
                }
                else if (method.Parameters.Length == 2)
                {
                    entityType = method.Parameters[1].Type;
                }
            }

            if (isWriter && writerMethodName is null)
            {
                writerMethodName = method.Name;
                // entity type: first param when void Write(T source, Span<byte> dest)
                if (method.ReturnsVoid && method.Parameters.Length == 2 && entityType is null)
                {
                    entityType = method.Parameters[0].Type;
                }
            }
        }

        if (entityType is null || readerMethodName is null || writerMethodName is null)
        {
            return null;
        }

        // Resolve map size from [Map] on entityType
        var mapAttr = entityType.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
        var size = mapAttr is not null && mapAttr.ConstructorArguments.Length > 0
            ? (int)(mapAttr.ConstructorArguments[0].Value ?? 0)
            : -1;
        if (size <= 0) return null;

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToDisplayString();

        var rootNs = DetermineRootNamespace(classSymbol);

        return new EndpointModel(
            ns,
            classSymbol.Name,
            entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            readerMethodName,
            writerMethodName,
            size,
            profileKey,
            generateArray,
            rootNs);
    }

    private static string DetermineRootNamespace(INamedTypeSymbol symbol)
    {
        // Walk up the namespace chain to find root
        var ns = symbol.ContainingNamespace;
        if (ns.IsGlobalNamespace) return string.Empty;

        var parts = new List<string>();
        while (!ns.IsGlobalNamespace)
        {
            parts.Insert(0, ns.Name);
            ns = ns.ContainingNamespace;
        }

        return parts[0]; // root segment
    }

    // -------------------------------------------------------
    // Execute
    // -------------------------------------------------------

    private static void Execute(
        SourceProductionContext spc,
        ImmutableArray<EndpointModel> endpoints)
    {
        if (endpoints.IsDefaultOrEmpty) return;

        // 1) Per-class: emit CreateByteMapperBinding / CreateByteMapperArrayBinding
        foreach (var ep in endpoints)
        {
            var source = BuildBindingSource(ep);
            var filename = MakeFilename(ep.Namespace, ep.ClassName) + ".AspNetCore.g.cs";
            spc.AddSource(filename, SourceText.From(source, Encoding.UTF8));
        }

        // 2) Assembly-level bootstrap + AddByteMapperFormatters
        var bootstrap = BuildBootstrapSource(endpoints);
        spc.AddSource("__ByteMapperAspNetCoreBootstrap.g.cs", SourceText.From(bootstrap, Encoding.UTF8));
    }

    // -------------------------------------------------------
    // Source builders
    // -------------------------------------------------------

    private static string BuildBindingSource(EndpointModel ep)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(ep.Namespace))
        {
            sb.AppendLine($"namespace {ep.Namespace};");
            sb.AppendLine();
        }

        sb.AppendLine($"partial class {ep.ClassName}");
        sb.AppendLine("{");

        // Single binding factory
        sb.AppendLine($"    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding<{ep.EntityTypeFqn}> CreateByteMapperBinding()");
        sb.AppendLine("        => new(");
        sb.AppendLine($"            size: {ep.Size},");
        sb.AppendLine($"            read:  static (s, t) => {ep.ReaderMethodName}(s, t),");
        sb.AppendLine($"            write: static (s, d) => {ep.WriterMethodName}(s, d),");
        sb.AppendLine($"            factory: static () => new {ep.EntityTypeFqn}());");
        sb.AppendLine();

        if (ep.GenerateArrayBinding)
        {
            // Array binding factory
            sb.AppendLine($"    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperArrayBinding<{ep.EntityTypeFqn}> CreateByteMapperArrayBinding()");
            sb.AppendLine("        => new(");
            sb.AppendLine($"            elementSize: {ep.Size},");
            sb.AppendLine($"            readElement:  static (s, t) => {ep.ReaderMethodName}(s, t),");
            sb.AppendLine($"            writeElement: static (s, d) => {ep.WriterMethodName}(s, d),");
            sb.AppendLine($"            factory:      static () => new {ep.EntityTypeFqn}());");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildBootstrapSource(ImmutableArray<EndpointModel> endpoints)
    {
        // Use the root namespace of the first endpoint (or a fixed ns)
        var rootNs = endpoints.Select(e => e.RootNamespace).FirstOrDefault(n => !string.IsNullOrEmpty(n))
                     ?? "ByteMapperGenerated";

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine($"namespace {rootNs};");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
        sb.AppendLine("internal static class __ByteMapperAspNetCoreBootstrap");
        sb.AppendLine("{");
        sb.AppendLine("    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry Build()");
        sb.AppendLine("    {");
        sb.AppendLine("        var single = new Dictionary<(global::System.Type, string?), global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding>");
        sb.AppendLine("        {");
        foreach (var ep in endpoints)
        {
            var qualifiedClass = string.IsNullOrEmpty(ep.Namespace)
                ? $"global::{ep.ClassName}"
                : $"global::{ep.Namespace}.{ep.ClassName}";
            var profileLiteral = ep.ProfileKey is null ? "null" : $"\"{ep.ProfileKey}\"";
            sb.AppendLine($"            {{ (typeof({ep.EntityTypeFqn}), {profileLiteral}), {qualifiedClass}.CreateByteMapperBinding() }},");
        }
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        var array = new Dictionary<(global::System.Type, string?), object>");
        sb.AppendLine("        {");
        foreach (var ep in endpoints.Where(e => e.GenerateArrayBinding))
        {
            var qualifiedClass = string.IsNullOrEmpty(ep.Namespace)
                ? $"global::{ep.ClassName}"
                : $"global::{ep.Namespace}.{ep.ClassName}";
            var profileLiteral = ep.ProfileKey is null ? "null" : $"\"{ep.ProfileKey}\"";
            sb.AppendLine($"            {{ (typeof({ep.EntityTypeFqn}), {profileLiteral}), {qualifiedClass}.CreateByteMapperArrayBinding() }},");
        }
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("        return new global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry(single, array);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        // AddByteMapperFormatters extension emitted into user assembly
        sb.AppendLine("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
        sb.AppendLine($"internal static class __ByteMapperServiceCollectionExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Registers ByteMapper registry, formatters, and options. Generated by source generator.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddByteMapperFormatters(");
        sb.AppendLine("        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,");
        sb.AppendLine("        global::System.Action<global::Smart.IO.ByteMapper.AspNetCore.ByteMapperFormatterOptions>? configure = null)");
        sb.AppendLine("        => global::Smart.IO.ByteMapper.AspNetCore.ByteMapperServiceCollectionExtensions.AddByteMapperFormatters(");
        sb.AppendLine("            services,");
        sb.AppendLine("            __ByteMapperAspNetCoreBootstrap.Build(),");
        sb.AppendLine("            configure);");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string MakeFilename(string ns, string className)
    {
        var buffer = new StringBuilder();
        if (!string.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }
        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        return buffer.ToString();
    }

    // -------------------------------------------------------
    // Model
    // -------------------------------------------------------

    private sealed record EndpointModel(
        string Namespace,
        string ClassName,
        string EntityTypeFqn,
        string ReaderMethodName,
        string WriterMethodName,
        int Size,
        string? ProfileKey,
        bool GenerateArrayBinding,
        string RootNamespace);
}
