namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Smart.IO.ByteMapper.Generator;

// Roslyn incremental generator をインメモリで実行し、生成ソースを返すヘルパー。
// core と AspNetCore の両ジェネレーターを実行するため、mapper 実装とバインディングの両方が生成される。
internal static class AspNetCoreGeneratorTestHelper
{
    private static readonly Assembly SmartByteMapperAssembly =
        typeof(MapAttribute).Assembly;

    private static readonly Assembly SmartByteMapperAspNetCoreAssembly =
        typeof(ByteMapperEndpointAttribute).Assembly;

    // ジェネレーターが依存する SourceGenerateHelper を事前ロードしておく。
    // テスト実行バイナリと同一ディレクトリに配置されているため、
    // Assembly.GetExecutingAssembly の Location から解決する。
    private static readonly Lazy<bool> EnsureDeps = new(() =>
    {
        var dir = Path.GetDirectoryName(typeof(AspNetCoreGeneratorTestHelper).Assembly.Location)!;
        var helper = Path.Combine(dir, "SourceGenerateHelper.dll");
        if (File.Exists(helper))
        {
            Assembly.LoadFrom(helper);
        }
        return true;
    });

    // 指定ソースコードに対してジェネレーターを実行し、生成された全ソーステキストを返す。
    public static IReadOnlyList<string> GetGeneratedSources(string source)
    {
        _ = EnsureDeps.Value;
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            MetadataReference.CreateFromFile(SmartByteMapperAssembly.Location),
            MetadataReference.CreateFromFile(SmartByteMapperAspNetCoreAssembly.Location)
        }.Concat(GetRuntimeReferences());

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators:
            [
                new ByteMapperGenerator().AsSourceGenerator(),
                new ByteMapperAspNetCoreGenerator().AsSourceGenerator()
            ],
            parseOptions: (CSharpParseOptions)syntaxTree.Options);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        return driver.GetRunResult().Results
            .SelectMany(static r => r.GeneratedSources)
            .Select(static s => s.SourceText.ToString())
            .ToList();
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
