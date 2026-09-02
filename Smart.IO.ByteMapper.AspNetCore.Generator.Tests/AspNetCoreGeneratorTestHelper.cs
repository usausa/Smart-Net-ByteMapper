namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.Generator;

using SourceGenerateHelper.Testing;

internal static class AspNetCoreGeneratorTestHelper
{
    private static GeneratorTestRunner Runner => new GeneratorTestRunner(
            new ByteMapperGenerator(),
            new ByteMapperAspNetCoreGenerator())
        .WithReference(typeof(MapAttribute).Assembly)
        .WithReference(typeof(ByteMapperEndpointAttribute).Assembly);

    public static IReadOnlyList<string> GetGeneratedSources(string source) =>
        Runner.Run(source).GeneratedSources.Values.ToArray();

    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) =>
        Runner.WithDiagnosticPrefix("SBM").GetDiagnostics(source);

    public static IncrementalRunResult RunIncremental(string source, string addedSource) =>
        Runner.WithTracking().RunIncremental(source, addedSource);
}
