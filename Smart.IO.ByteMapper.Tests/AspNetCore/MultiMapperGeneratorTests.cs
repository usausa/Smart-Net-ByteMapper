namespace Smart.IO.ByteMapper.AspNetCore;

using System;

// ── Entity and profile types for multi-mapper generator test ──────────────────

[Map(4, UseDelimiter = false)]
internal sealed class MultiMapEntity
{
    [MapBinary<int>(0)]
    public int Id { get; set; }
}

#pragma warning disable CA1812
[Map(4, UseDelimiter = false)]
internal sealed class MultiMapProfile
{
    [MapBinary<int>(0)]
    public int Id { get; set; }
}
#pragma warning restore CA1812

// ── Mapper with two reader/writer pairs (default + profile) ───────────────────

[ByteMapperEndpoint]
internal static partial class MultiMapEndpoint
{
    [ByteReader]
    public static partial void ReadDefault(ReadOnlySpan<byte> src, MultiMapEntity target);

    [ByteWriter]
    public static partial void WriteDefault(Span<byte> dst, MultiMapEntity source);

    [ByteReader(Profile = typeof(MultiMapProfile))]
    public static partial void ReadProfile(ReadOnlySpan<byte> src, MultiMapEntity target);

    [ByteWriter(Profile = typeof(MultiMapProfile))]
    public static partial void WriteProfile(Span<byte> dst, MultiMapEntity source);
}

// ── Tests ─────────────────────────────────────────────────────────────────────

public class MultiMapperGeneratorTests
{
    [Fact]
    public void WhenTwoBindingsThenDefaultFactoryMethodExistsAndHasCorrectSize()
    {
        var binding = MultiMapEndpoint.CreateByteMapperBinding();

        Assert.Equal(4, binding.Size);
    }

    [Fact]
    public void WhenTwoBindingsThenProfileFactoryMethodExistsAndHasCorrectSize()
    {
        var binding = MultiMapEndpoint.CreateByteMapperBinding_MultiMapProfile();

        Assert.Equal(4, binding.Size);
    }

    [Fact]
    public void WhenTwoBindingsThenDefaultAndProfileAreDistinctInstances()
    {
        var defaultBinding = MultiMapEndpoint.CreateByteMapperBinding();
        var profileBinding = MultiMapEndpoint.CreateByteMapperBinding_MultiMapProfile();

        Assert.NotSame(defaultBinding, profileBinding);
    }

    [Fact]
    public void WhenDefaultBindingThenRoundTripPreservesId()
    {
        var binding = MultiMapEndpoint.CreateByteMapperBinding();
        var buffer = new byte[4];
        var original = new MultiMapEntity { Id = 0x01020304 };
        binding.Write(original, buffer);

        var result = new MultiMapEntity();
        binding.Read(buffer, result);

        Assert.Equal(original.Id, result.Id);
    }

    [Fact]
    public void WhenProfileBindingThenRoundTripPreservesId()
    {
        var binding = MultiMapEndpoint.CreateByteMapperBinding_MultiMapProfile();
        var buffer = new byte[4];
        var original = new MultiMapEntity { Id = 0x0A0B0C0D };
        binding.Write(original, buffer);

        var result = new MultiMapEntity();
        binding.Read(buffer, result);

        Assert.Equal(original.Id, result.Id);
    }
}
