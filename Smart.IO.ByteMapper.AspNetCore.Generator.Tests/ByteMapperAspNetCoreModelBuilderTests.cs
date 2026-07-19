namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using System.Linq;

// Driver-based tests for the parse stage. The regression target is size resolution: a profile
// endpoint must take its record size from the profile's [MapProfile], not from the entity's [Map].
public class ByteMapperAspNetCoreModelBuilderTests
{
    private const string Source = """
        using System;
        using Smart.IO.ByteMapper;
        using Smart.IO.ByteMapper.AspNetCore;

        namespace Test;

        [Map(59)]
        public sealed class SampleData
        {
            [MapText(0, 13)]
            public string Code { get; set; } = default!;

            [MapText(13, 20)]
            public string Name { get; set; } = default!;
        }

        [MapProfile(35)]
        [MapTextMember(nameof(SampleData.Code), 0, 13)]
        [MapTextMember(nameof(SampleData.Name), 13, 20)]
        public sealed class SampleDataCodeNameProfile
        {
        }

        [ByteMapperEndpoint]
        public static partial class SampleDataMappers
        {
            [ByteReader]
            public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

            [ByteWriter]
            public static partial void Write(Span<byte> destination, SampleData source);
        }

        [ByteMapperEndpoint]
        public static partial class SampleDataCodeNameMappers
        {
            [ByteReader(Profile = typeof(SampleDataCodeNameProfile))]
            public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

            [ByteWriter(Profile = typeof(SampleDataCodeNameProfile))]
            public static partial void Write(Span<byte> destination, SampleData source);
        }
        """;

    private static string FindBindingSource(string className)
    {
        var sources = AspNetCoreGeneratorTestHelper.GetGeneratedSources(Source);
        return sources.Single(s =>
            s.Contains($"partial class {className}", StringComparison.Ordinal)
            && s.Contains("ByteMapperBinding<", StringComparison.Ordinal));
    }

    [Fact]
    public void WhenProfileEndpointThenBindingUsesMapProfileSize()
    {
        var binding = FindBindingSource("SampleDataCodeNameMappers");

        Assert.Contains("size: 35,", binding, StringComparison.Ordinal);
        Assert.Contains("elementSize:  35,", binding, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenDefaultEndpointThenBindingUsesMapSize()
    {
        var binding = FindBindingSource("SampleDataMappers");

        Assert.Contains("size: 59,", binding, StringComparison.Ordinal);
        Assert.Contains("elementSize:  59,", binding, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenProfileEndpointThenRegisteredWithProfileType()
    {
        var sources = AspNetCoreGeneratorTestHelper.GetGeneratedSources(Source);
        var bootstrap = sources.Single(static s => s.Contains("__ByteMapperAspNetCoreBootstrap", StringComparison.Ordinal));

        Assert.Contains("(typeof(global::Test.SampleData), typeof(global::Test.SampleDataCodeNameProfile))", bootstrap, StringComparison.Ordinal);
    }
}
