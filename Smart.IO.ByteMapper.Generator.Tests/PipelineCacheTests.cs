namespace Smart.IO.ByteMapper.Generator.Tests;

using SourceGenerateHelper.Testing;

public sealed class PipelineCacheTests
{
    private const string Source =
        """
        using System;
        using Smart.IO.ByteMapper;

        namespace Test;

        [Map(36, UseDelimiter = false)]
        public sealed class SampleRecord
        {
            [MapBinary<int>(0)]
            public int Id { get; set; }

            [MapText(4, 32)]
            public string Name { get; set; } = default!;
        }

        public static partial class SampleMappers
        {
            [ByteReader]
            public static partial void Read(ReadOnlySpan<byte> buffer, SampleRecord target);
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

        namespace Test;

        public static partial class AddedMappers
        {
            [ByteWriter]
            public static partial void Write(Span<byte> buffer, SampleRecord source);
        }
        """;

    // ------------------------------------------------------------
    // Cache
    // ------------------------------------------------------------

    [Fact]
    public void UnrelatedEditKeepsModelCached()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, UnrelatedSource);

        // Assert
        Assert.Equal(result.FirstGeneratedText, result.SecondGeneratedText);
        Assert.NotEmpty(result.OutputReasons);
        Assert.DoesNotContain(result.OutputReasons, static x => x.IsChanged());
    }

    [Fact]
    public void TargetEditRebuildsModel()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, AddedTargetSource);

        // Assert
        Assert.Contains(result.OutputReasons, static x => x.IsChanged());
    }
}
