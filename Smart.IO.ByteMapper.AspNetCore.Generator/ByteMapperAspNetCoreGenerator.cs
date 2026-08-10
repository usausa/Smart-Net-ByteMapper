namespace Smart.IO.ByteMapper.AspNetCore.Generator;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// Incremental generator orchestrator. Parsing lives in ByteMapperAspNetCoreModelBuilder and source
// emission in ByteMapperAspNetCoreSourceBuilder; this type only wires the pipeline.
[Generator]
public sealed class ByteMapperAspNetCoreGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var parsed = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperAspNetCoreModelBuilder.ByteMapperEndpointAttributeName,
                static (s, _) => s is ClassDeclarationSyntax,
                static (ctx, _) => ByteMapperAspNetCoreModelBuilder.ParseEndPoints(ctx));

        // 診断はライブ表示のため RegisterSourceOutput 側へ分離する
        context.RegisterSourceOutput(
            parsed.Collect(),
            static (spc, results) => ReportDiagnostics(spc, results));

        var endPoints = parsed.SelectMany(static (result, _) => result.Value.EndPoints);

        // 生成は per-endPoint（1 endPoint = 1 ファイル）
        context.RegisterImplementationSourceOutput(
            endPoints,
            static (spc, ep) => Execute(spc, ep));

        // ブートストラップは全 endPoint の集約なので Collect のまま単一出力
        context.RegisterImplementationSourceOutput(
            endPoints.Collect(),
            static (spc, items) => ExecuteBootstrap(spc, items));
    }

    private static void ReportDiagnostics(SourceProductionContext spc, ImmutableArray<Result<EndPointCollection>> results)
    {
        foreach (var diagnostic in results.SelectError())
        {
            spc.ReportDiagnostic(diagnostic);
        }
    }

    private static void Execute(SourceProductionContext spc, EndPointModel ep)
    {
        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBinding(builder, ep);
        // NameSuffix arrives already '_'-prefixed (or empty); HintNameBuilder inserts the separator
        // itself, so trim it before handing the parts over.
        spc.AddSource(
            HintNameBuilder.BuildWithExtension(
                ep.Namespace, ".AspNetCore.g.cs", ep.ClassName, ep.NameSuffix.TrimStart('_')),
            builder);
    }

    private static void ExecuteBootstrap(
        SourceProductionContext spc,
        ImmutableArray<EndPointModel> endPoints)
    {
        if (endPoints.IsDefaultOrEmpty)
        {
            return;
        }

        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBootstrap(builder, endPoints);
        spc.AddSource("__ByteMapperAspNetCoreBootstrap.g.cs", builder);
    }
}
