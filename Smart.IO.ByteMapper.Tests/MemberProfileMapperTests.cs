namespace Smart.IO.ByteMapper;

using System;

// Member-less profiles using [MapProfile] + [Map...Member] class attributes.
// These reuse the ProfileTarget entity from ProfileMapperTests and must behave identically to the
// property-based profiles defined there.

// ---- Member profile A: mirrors ProfileA layout exactly (20 bytes) ----
// Id(0,4) + Code(4,8) + Active(12,1) + Label(13,7)

#pragma warning disable CA1812
[MapProfile(20, UseDelimiter = false)]
[MapBinaryMember<int>(nameof(ProfileTarget.Id), 0)]
[MapTextMember(nameof(ProfileTarget.Code), 4, 8)]
[MapBooleanMember(nameof(ProfileTarget.Active), 12)]
[MapTextMember(nameof(ProfileTarget.Label), 13, 7)]
internal sealed class MemberProfileA
{
}
#pragma warning restore CA1812

// ---- Option profiles: same MapText options expressed both ways (8 bytes, Code only) ----

#pragma warning disable CA1812
[Map(8, UseDelimiter = false)]
internal sealed class OptPropertyProfile
{
    [MapText(0, 8, Padding = Padding.Left, Filler = 0x2A)]
    public string Code { get; set; } = default!;
}
#pragma warning restore CA1812

#pragma warning disable CA1812
[MapProfile(8, UseDelimiter = false)]
[MapTextMember(nameof(ProfileTarget.Code), 0, 8, Padding = Padding.Left, Filler = 0x2A)]
internal sealed class OptMemberProfile
{
}
#pragma warning restore CA1812

// ---- Mappers ----

internal static partial class MemberProfileMappers
{
    [ByteReader(Profile = typeof(MemberProfileA))]
    public static partial void ReadA(ReadOnlySpan<byte> buffer, ProfileTarget target);

    [ByteWriter(Profile = typeof(MemberProfileA))]
    public static partial void WriteA(Span<byte> buffer, ProfileTarget source);

    [ByteReader(Profile = typeof(MemberProfileA))]
    public static partial ProfileTarget ReadANew(ReadOnlySpan<byte> buffer);

    [ByteWriter(Profile = typeof(MemberProfileA))]
    public static partial byte[] WriteAAlloc(ProfileTarget source);

    [ByteWriter(Profile = typeof(OptPropertyProfile))]
    public static partial void WriteOptProperty(Span<byte> buffer, ProfileTarget source);

    [ByteWriter(Profile = typeof(OptMemberProfile))]
    public static partial void WriteOptMember(Span<byte> buffer, ProfileTarget source);
}

// ---- Tests: parity with the property-based ProfileA ----

public class MemberProfileParityTests
{
    [Fact]
    public void WhenWriteThenBytesMatchPropertyProfile()
    {
        var target = new ProfileTarget { Id = 100, Code = "ITEM", Active = true, Label = "Hello" };

        var memberBuffer = new byte[20];
        var propertyBuffer = new byte[20];
        MemberProfileMappers.WriteA(memberBuffer, target);
        ProfileMappers.WriteA(propertyBuffer, target);

        Assert.Equal(propertyBuffer, memberBuffer);
    }

    [Fact]
    public void WhenWriteAllocThenBytesMatchPropertyProfile()
    {
        var target = new ProfileTarget { Id = -1, Code = "ABCDEFGH", Active = false, Label = "World!!" };

        var memberBytes = MemberProfileMappers.WriteAAlloc(target);
        var propertyBytes = ProfileMappers.WriteAAlloc(target);

        Assert.Equal(propertyBytes, memberBytes);
    }

    [Fact]
    public void WhenReadNewThenValuesMatchPropertyProfile()
    {
        var target = new ProfileTarget { Id = 12345, Code = "XY", Active = true, Label = "Lbl" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        var viaMember = MemberProfileMappers.ReadANew(buffer);
        var viaProperty = ProfileMappers.ReadANew(buffer);

        Assert.Equal(viaProperty.Id, viaMember.Id);
        Assert.Equal(viaProperty.Code, viaMember.Code);
        Assert.Equal(viaProperty.Active, viaMember.Active);
        Assert.Equal(viaProperty.Label, viaMember.Label);
    }

    [Fact]
    public void WhenRoundTripThenValuesAreRestored()
    {
        var target = new ProfileTarget { Id = 7, Code = "DATA", Active = false, Label = "Round" };

        var buffer = new byte[20];
        MemberProfileMappers.WriteA(buffer, target);
        var restored = new ProfileTarget();
        MemberProfileMappers.ReadA(buffer, restored);

        Assert.Equal(target.Id, restored.Id);
        Assert.Equal(target.Code, restored.Code);
        Assert.Equal(target.Active, restored.Active);
        Assert.Equal(target.Label, restored.Label);
    }

    [Fact]
    public void WhenMemberCarriesOptionsThenBytesMatchPropertyOptions()
    {
        var target = new ProfileTarget { Code = "AB" };

        var memberBuffer = new byte[8];
        var propertyBuffer = new byte[8];
        MemberProfileMappers.WriteOptMember(memberBuffer, target);
        MemberProfileMappers.WriteOptProperty(propertyBuffer, target);

        // Padding.Left + Filler '*' => "******AB"
        Assert.Equal(propertyBuffer, memberBuffer);
        Assert.Equal("******AB"u8.ToArray(), memberBuffer);
    }
}
