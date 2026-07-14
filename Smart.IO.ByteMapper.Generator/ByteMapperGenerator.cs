namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

// Incremental generator orchestrator. Parsing lives in ByteMapperModelBuilder and source emission in
// ByteMapperSourceBuilder; this type only wires the pipeline and reports diagnostics.
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
                .Select(static g => new ClassModel(g.Key.Namespace, g.Key.ClassName, new EquatableArray<MapperMethodModel>(g.ToArray())))
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

        // Report per-method diagnostics / メソッドごとの診断情報を報告する
        foreach (var m in results.SelectValue())
        {
            foreach (var err in m.Errors)
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

        var filename = MakeFilename(group.Namespace, group.ClassName);
        context.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private static string MakeFilename(string ns, string className)
    {
        var buffer = new StringBuilder();
        if (!String.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }
        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        buffer.Append(".g.cs");
        return buffer.ToString();
    }
}
