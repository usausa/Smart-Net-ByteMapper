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
            .SelectMany(static (arr, _) => arr);

        // 生成は per-endpoint（1 endpoint = 1 ファイル）
        context.RegisterImplementationSourceOutput(
            endpoints,
            static (spc, ep) => Execute(spc, ep));

        // ブートストラップは全 endpoint の集約なので Collect のまま単一出力
        context.RegisterImplementationSourceOutput(
            endpoints.Collect(),
            static (spc, items) => ExecuteBootstrap(spc, items));
    }

    private static void Execute(SourceProductionContext spc, EndpointModel ep)
    {
        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBinding(builder, ep);
        var filename = ByteMapperAspNetCoreSourceBuilder.MakeFilename(ep.Namespace, ep.ClassName, ep.NameSuffix) + ".AspNetCore.g.cs";
        spc.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private static void ExecuteBootstrap(
        SourceProductionContext spc,
        ImmutableArray<EndpointModel> endpoints)
    {
        if (endpoints.IsDefaultOrEmpty)
        {
            return;
        }

        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBootstrap(builder, endpoints);
        spc.AddSource("__ByteMapperAspNetCoreBootstrap.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }
}
