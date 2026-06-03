namespace Smart.IO.ByteMapper.Generator.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

// Roslyn incremental generator をインメモリで実行し、診断結果を返すヘルパー
internal static class GeneratorTestHelper
{
    private static readonly Assembly SmartByteMapperAssembly =
        typeof(Smart.IO.ByteMapper.MapAttribute).Assembly;

    // ジェネレーターが依存する SourceGenerateHelper を事前ロードしておく。
    // テスト実行バイナリと同一ディレクトリに配置されているため、
    // Assembly.GetExecutingAssembly の Location から解決する。
    private static readonly Lazy<bool> EnsureDeps = new(() =>
    {
        var dir = Path.GetDirectoryName(typeof(GeneratorTestHelper).Assembly.Location)!;
        var helper = Path.Combine(dir, "SourceGenerateHelper.dll");
        if (File.Exists(helper))
        {
            Assembly.LoadFrom(helper);
        }
        return true;
    });

    // 指定ソースコードに対してジェネレーターを実行し、SBM 診断を返す。
    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) =>
        RunGenerator(source)
            .Where(static d => d.Id.StartsWith("SBM", StringComparison.Ordinal))
            .ToList();

    // SBM 以外の診断も含め全診断を返す（デバッグ用）。
    public static IReadOnlyList<Diagnostic> GetDiagnosticsAll(string source) =>
        RunGenerator(source).ToList();

    private static IEnumerable<Diagnostic> RunGenerator(string source)
    {
        _ = EnsureDeps.Value;
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            MetadataReference.CreateFromFile(SmartByteMapperAssembly.Location)
        }.Concat(GetRuntimeReferences());

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new ByteMapperGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            parseOptions: (CSharpParseOptions)syntaxTree.Options);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var outputCompilation, out var generatorDiagnostics);

        var driverResult = driver.GetRunResult();

        return driverResult.Results
            .SelectMany(static r => r.Diagnostics)
            .Concat(generatorDiagnostics)
            .Concat(outputCompilation.GetDiagnostics());
    }

    private static IEnumerable<MetadataReference> GetRuntimeReferences()
    {
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is not string trustedAssemblies)
        {
            yield break;
        }

        foreach (var path in trustedAssemblies.Split(Path.PathSeparator))
        {
            if (!String.IsNullOrEmpty(path))
            {
                yield return MetadataReference.CreateFromFile(path);
            }
        }
    }
}
