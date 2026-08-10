namespace Smart.IO.ByteMapper.Generator.Tests;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper.Testing;

internal static class GeneratorTestHelper
{
    private static GeneratorTestRunner Runner => GeneratorTestRunner
        .For<ByteMapperGenerator>()
        .WithReference(typeof(MapAttribute).Assembly)
        .WithReference(typeof(MapFastDateTimeAttribute).Assembly)
        .WithDiagnosticPrefix("SBM");

    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) => Runner.GetDiagnostics(source);

    public static IReadOnlyList<Diagnostic> GetDiagnosticsAll(string source) => Runner.GetDiagnosticsAll(source);
}
