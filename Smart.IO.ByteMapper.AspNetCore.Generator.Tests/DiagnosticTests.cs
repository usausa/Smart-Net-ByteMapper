namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

public class DiagnosticTests
{
    private const string Entity = """
        using System;
        using Smart.IO.ByteMapper;
        using Smart.IO.ByteMapper.AspNetCore;

        namespace Test;

        [Map(33)]
        public sealed class SampleData
        {
            [MapText(0, 13)]
            public string Code { get; set; } = default!;

            [MapText(13, 20)]
            public string Name { get; set; } = default!;
        }

        public sealed class Unmapped
        {
            public string Code { get; set; } = default!;
        }
        """;

    //-----------------------------------------------------------------------
    // SBM1001 : a reader has no matching writer
    //-----------------------------------------------------------------------

    [Fact]
    public void Sbm1001ReaderWithoutWriterEmitsDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);
            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "SBM1001");
    }

    //-----------------------------------------------------------------------
    // SBM1002 : the entity size cannot be resolved
    //-----------------------------------------------------------------------

    [Fact]
    public void Sbm1002UnknownEntitySizeEmitsDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class UnmappedMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, Unmapped target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, Unmapped source);
            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "SBM1002");
    }

    //-----------------------------------------------------------------------
    // A complete pair must stay clean
    //-----------------------------------------------------------------------

    [Fact]
    public void ValidEndPointEmitsNoDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """);

        Assert.Empty(diagnostics);
    }
}
