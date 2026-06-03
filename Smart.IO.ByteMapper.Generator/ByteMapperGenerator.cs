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

        context.RegisterImplementationSourceOutput(
            methods,
            static (spc, items) => Execute(spc, items));
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<Result<MapperMethodModel>> results)
    {
        foreach (var error in results.SelectError())
        {
            context.ReportDiagnostic(error);
        }

        var builder = new SourceBuilder();

        var methodsByClass = results.SelectValue()
            .GroupBy(m => new { m.Namespace, m.ClassName });

        foreach (var group in methodsByClass)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            // Report per-method diagnostics / メソッドごとの診断情報を報告する
            foreach (var m in group)
            {
                foreach (var err in m.Errors)
                {
                    context.ReportDiagnostic(err);
                }
            }

            builder.Clear();
            ByteMapperSourceBuilder.Build(builder, group.ToList());

            var filename = MakeFilename(group.Key.Namespace, group.Key.ClassName);
            context.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
        }
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
