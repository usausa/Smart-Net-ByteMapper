namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// Incremental generator orchestrator. Parsing lives in ByteMapperAspNetCoreModelBuilder and source
// emission in ByteMapperAspNetCoreSourceBuilder; this type only wires the pipeline.
[Generator]
public sealed class ByteMapperAspNetCoreGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var endpoints = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperAspNetCoreModelBuilder.ByteMapperEndpointAttributeName,
                static (s, _) => s is ClassDeclarationSyntax,
                static (ctx, _) => ByteMapperAspNetCoreModelBuilder.ParseEndpoints(ctx))
            .Where(static arr => arr.Count > 0)
            .SelectMany(static (arr, _) => arr)
            .Collect();

        context.RegisterImplementationSourceOutput(
            endpoints,
            static (spc, items) => Execute(spc, items));
    }

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
            ByteMapperAspNetCoreSourceBuilder.BuildBinding(builder, ep);
            var filename = ByteMapperAspNetCoreSourceBuilder.MakeFilename(ep.Namespace, ep.ClassName, ep.NameSuffix) + ".AspNetCore.g.cs";
            spc.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        builder.Clear();
        ByteMapperAspNetCoreSourceBuilder.BuildBootstrap(builder, endpoints);
        spc.AddSource("__ByteMapperAspNetCoreBootstrap.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }
}
