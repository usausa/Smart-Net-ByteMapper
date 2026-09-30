namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

[Generator]
public sealed class ByteMapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var readers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperModelBuilder.ByteReaderAttributeName,
                static (s, _) => s is MethodDeclarationSyntax,
                static (ctx, _) => ByteMapperModelBuilder.Parse(ctx, MapperKind.Reader))
            .Collect();

        var writers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteMapperModelBuilder.ByteWriterAttributeName,
                static (s, _) => s is MethodDeclarationSyntax,
                static (ctx, _) => ByteMapperModelBuilder.Parse(ctx, MapperKind.Writer))
            .Collect();

        var methods = readers.Combine(writers)
            .Select(static (t, _) => t.Left.AddRange(t.Right));

        var readerTrees = context.ForAttributeWithMetadataNameSyntaxTrees(
            ByteMapperModelBuilder.ByteReaderAttributeName,
            static (s, _) => s is MethodDeclarationSyntax);
        var writerTrees = context.ForAttributeWithMetadataNameSyntaxTrees(
            ByteMapperModelBuilder.ByteWriterAttributeName,
            static (s, _) => s is MethodDeclarationSyntax);
        var trees = readerTrees.Combine(writerTrees)
            .Select(static (t, _) => t.Left.AddRange(t.Right));

        context.RegisterSourceOutput(
            methods.Combine(trees),
            static (spc, items) => ReportDiagnostics(spc, items.Left, items.Right));

        var groups = methods.SelectMany(static (results, _) => SelectClasses(results));
        context.RegisterImplementationSourceOutput(
            groups,
            static (spc, group) => Execute(spc, group));
    }

    private static void ReportDiagnostics(SourceProductionContext context, ImmutableArray<Result<MapperMethodModel>> results, ImmutableArray<SyntaxTree> trees)
    {
        var diagnostics = results.SelectError()
            .Concat(results.SelectValue().SelectMany(static x => x.Diagnostics))
            .Concat(FindHintNameCollisions(results).Values)
            .Distinct();
        context.ReportDiagnostics(diagnostics, trees);
    }

    private static ImmutableArray<ClassModel> SelectClasses(ImmutableArray<Result<MapperMethodModel>> results)
    {
        var collisions = FindHintNameCollisions(results);
        return results.SelectValue()
            .Where(x => !collisions.ContainsKey(x.HintName))
            .GroupBy(static x => x.HintName)
            .Select(static x => new ClassModel(x.Key, new EquatableArray<MapperMethodModel>(x)))
            .ToImmutableArray();
    }

    private static Dictionary<string, DiagnosticInfo> FindHintNameCollisions(ImmutableArray<Result<MapperMethodModel>> results)
    {
        var collisions = new Dictionary<string, DiagnosticInfo>(StringComparer.Ordinal);
        var firsts = new Dictionary<string, MapperMethodModel>(StringComparer.OrdinalIgnoreCase);
        foreach (var method in results.SelectValue().OrderBy(static x => x.HintName, StringComparer.Ordinal))
        {
            if (!firsts.TryGetValue(method.HintName, out var first))
            {
                firsts.Add(method.HintName, method);
            }
            else if ((first.HintName != method.HintName) && !collisions.ContainsKey(method.HintName))
            {
                collisions.Add(method.HintName, new DiagnosticInfo(Diagnostics.HintNameCollision, (Location?)null, method.TypeName, first.TypeName));
            }
        }

        return collisions;
    }

    private static void Execute(SourceProductionContext context, ClassModel group)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builder = new SourceBuilder();
        ByteMapperSourceBuilder.Build(builder, group.Methods.ToList());

        context.AddSource(group.HintName, builder);
    }
}
