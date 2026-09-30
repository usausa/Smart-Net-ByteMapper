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

        var trees = context.ForAttributeWithMetadataNameSyntaxTrees(
            ByteMapperAspNetCoreModelBuilder.ByteMapperEndpointAttributeName,
            static (s, _) => s is ClassDeclarationSyntax);

        // 診断はライブ表示のため RegisterSourceOutput 側へ分離する
        var collected = parsed.Collect();
        context.RegisterSourceOutput(
            collected.Combine(trees),
            static (spc, results) => spc.ReportDiagnostics(
                results.Left.SelectError().Concat(FindHintNameCollisions(results.Left).Values).Distinct(),
                results.Right));

        var endPoints = collected.SelectMany(static (results, _) => SelectEndPoints(results));

        // 生成は per-endPoint（1 endPoint = 1 ファイル）
        context.RegisterImplementationSourceOutput(
            endPoints,
            static (spc, ep) => Execute(spc, ep));

        // ブートストラップは全 endPoint の集約なので Collect のまま単一出力
        context.RegisterImplementationSourceOutput(
            endPoints.Collect(),
            static (spc, items) => ExecuteBootstrap(spc, items));
    }

    private static IEnumerable<EndPointModel> SelectEndPoints(ImmutableArray<Result<EndPointCollection>> results)
    {
        var collisions = FindHintNameCollisions(results);
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var ep in results.SelectValue().SelectMany(static x => x.EndPoints))
        {
            if (!collisions.ContainsKey(ep.HintName) && emitted.Add(ep.HintName))
            {
                yield return ep;
            }
        }
    }

    private static Dictionary<string, DiagnosticInfo> FindHintNameCollisions(ImmutableArray<Result<EndPointCollection>> results)
    {
        var collisions = new Dictionary<string, DiagnosticInfo>(StringComparer.Ordinal);
        var firsts = new Dictionary<string, EndPointModel>(StringComparer.OrdinalIgnoreCase);
        foreach (var ep in results.SelectValue().SelectMany(static x => x.EndPoints).OrderBy(static x => x.HintName, StringComparer.Ordinal))
        {
            if (!firsts.TryGetValue(ep.HintName, out var first))
            {
                firsts.Add(ep.HintName, ep);
            }
            else if ((first.HintName != ep.HintName) && !collisions.ContainsKey(ep.HintName))
            {
                collisions.Add(ep.HintName, new DiagnosticInfo(Diagnostics.HintNameCollision, (Location?)null, TrimGlobal(ep.ClassFullName), TrimGlobal(first.ClassFullName)));
            }
        }

        return collisions;
    }

    private static string TrimGlobal(string name) =>
        name.StartsWith("global::", StringComparison.Ordinal) ? name.Substring("global::".Length) : name;

    private static void Execute(SourceProductionContext spc, EndPointModel ep)
    {
        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBinding(builder, ep);
        spc.AddSource(ep.HintName, builder);
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
