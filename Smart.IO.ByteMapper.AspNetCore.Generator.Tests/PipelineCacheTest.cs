namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using SourceGenerateHelper.Testing;

public sealed class PipelineCacheTest
{
    private const string Source =
        """
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

        [ByteMapperEndpoint]
        public static partial class SampleDataMappers
        {
            [ByteReader]
            public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

            [ByteWriter]
            public static partial void Write(Span<byte> destination, SampleData source);
        }
        """;

    private const string UnrelatedSource =
        """
        namespace Other;

        internal sealed class Unrelated;
        """;

    private const string AddedTargetSource =
        """
        using System;
        using Smart.IO.ByteMapper;
        using Smart.IO.ByteMapper.AspNetCore;

        namespace Test;

        [ByteMapperEndpoint]
        public static partial class AddedMappers
        {
            [ByteReader]
            public static partial void Read(ReadOnlySpan<byte> source, SampleData target);
        }
        """;

    // ------------------------------------------------------------
    // Cache
    // ------------------------------------------------------------

    [Fact]
    public void UnrelatedEditKeepsModelCached()
    {
        // Arrange & Act
        var result = AspNetCoreGeneratorTestHelper.RunIncremental(Source, UnrelatedSource);

        // Assert
        Assert.Equal(result.FirstGeneratedText, result.SecondGeneratedText);
        Assert.NotEmpty(result.OutputReasons);
        Assert.DoesNotContain(result.OutputReasons, static x => x.IsChanged());
    }

    [Fact]
    public void TargetEditRebuildsModel()
    {
        // Arrange & Act
        var result = AspNetCoreGeneratorTestHelper.RunIncremental(Source, AddedTargetSource);

        // Assert
        Assert.Contains(result.OutputReasons, static x => x.IsChanged());
    }
}
