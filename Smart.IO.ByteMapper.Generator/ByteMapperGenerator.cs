namespace Smart.IO.ByteMapper.Generator;

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

        context.RegisterSourceOutput(
            methods,
            static (spc, items) => ReportDiagnostics(spc, items));

        var groups = methods.SelectMany(static (results, _) =>
            results.SelectValue()
                .GroupBy(static m => new { m.Namespace, m.ClassName })
                .Select(static g => new ClassModel(g.Key.Namespace, g.Key.ClassName, new EquatableArray<MapperMethodModel>(g)))
                .ToImmutableArray());
        context.RegisterImplementationSourceOutput(
            groups,
            static (spc, group) => Execute(spc, group));
    }

    private static void ReportDiagnostics(SourceProductionContext context, ImmutableArray<Result<MapperMethodModel>> results)
    {
        foreach (var error in results.SelectError())
        {
            context.ReportDiagnostic(error);
        }

        foreach (var m in results.SelectValue())
        {
            foreach (var err in m.Diagnostics)
            {
                context.ReportDiagnostic(err);
            }
        }
    }

    private static void Execute(SourceProductionContext context, ClassModel group)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builder = new SourceBuilder();
        ByteMapperSourceBuilder.Build(builder, group.Methods.ToList());

        context.AddSource(HintNameBuilder.Build(group.Namespace, group.ClassName), builder);
    }
}
