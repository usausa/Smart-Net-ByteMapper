// ReSharper disable UseUtf8StringLiteral
#pragma warning disable IDE0230
namespace Smart.IO.ByteMapper;

using System;
using System.Text;

// ---- Target type (no [Map] or mapping attributes — layout comes from Profile) ----

internal sealed class ProfileTarget
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool? Active { get; set; }
}

// ---- Profile A: compact layout, 20 bytes total ----
// Id(0,4) + Code(4,8) + Active(12,1) + Label(13,7)

[Map(20, UseDelimiter = false)]
internal sealed class ProfileA
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 8)]
    public string Code { get; set; } = string.Empty;

    [MapBoolean(12)]
    public bool? Active { get; set; }

    [MapText(13, 7)]
    public string Label { get; set; } = string.Empty;
}

// ---- Profile B: wide layout, 40 bytes total ----
// Id(0,4) + Code(4,16) + Active(20,1) + Label(21,19)

[Map(40, UseDelimiter = false)]
internal sealed class ProfileB
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 16)]
    public string Code { get; set; } = string.Empty;

    [MapBoolean(20)]
    public bool? Active { get; set; }

    [MapText(21, 19)]
    public string Label { get; set; } = string.Empty;
}

// ---- Mappers using Profile ----

internal static partial class ProfileMappers
{
    // Profile A mappers
    [ByteReader(Profile = typeof(ProfileA))]
    public static partial void ReadA(ReadOnlySpan<byte> buffer, ProfileTarget target);

    [ByteWriter(Profile = typeof(ProfileA))]
    public static partial void WriteA(Span<byte> buffer, ProfileTarget source);

    [ByteReader(Profile = typeof(ProfileA))]
    public static partial ProfileTarget ReadANew(ReadOnlySpan<byte> buffer);

    [ByteWriter(Profile = typeof(ProfileA))]
    public static partial byte[] WriteAAlloc(ProfileTarget source);

    // Profile B mappers
    [ByteReader(Profile = typeof(ProfileB))]
    public static partial void ReadB(ReadOnlySpan<byte> buffer, ProfileTarget target);

    [ByteWriter(Profile = typeof(ProfileB))]
    public static partial void WriteB(Span<byte> buffer, ProfileTarget source);

    [ByteReader(Profile = typeof(ProfileB))]
    public static partial ProfileTarget ReadBNew(ReadOnlySpan<byte> buffer);

    [ByteWriter(Profile = typeof(ProfileB))]
    public static partial byte[] WriteBAlloc(ProfileTarget source);
}

// ---- Tests: Profile A ----

public class ProfileAMapperWriteTests
{
    [Fact]
    public void WhenWriteProfileAThenIdIsBigEndianAtOffset0()
    {
        var target = new ProfileTarget { Id = 100, Code = "ITEM", Active = true, Label = "Hello" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal(new byte[] { 0x00, 0x00, 0x00, 0x64 }, buffer[..4]);
    }

    [Fact]
    public void WhenWriteProfileAThenCodeIsAtOffset4()
    {
        var target = new ProfileTarget { Id = 1, Code = "ITEM", Active = true, Label = "X" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal("ITEM", Encoding.ASCII.GetString(buffer[4..8]));
    }

    [Fact]
    public void WhenWriteProfileAThenActiveTrueIsAtOffset12()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "Y" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal(0x31, buffer[12]);
    }

    [Fact]
    public void WhenWriteProfileAThenActiveFalseIsAtOffset12()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = false, Label = "Y" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal(0x30, buffer[12]);
    }

    [Fact]
    public void WhenWriteProfileAThenActiveNullIsSpaceAtOffset12()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = null, Label = "Y" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal(0x20, buffer[12]);
    }

    [Fact]
    public void WhenWriteProfileAThenLabelIsAtOffset13()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "TAG" };
        var buffer = new byte[20];
        ProfileMappers.WriteA(buffer, target);

        Assert.Equal("TAG", Encoding.ASCII.GetString(buffer[13..16]));
    }

    [Fact]
    public void WhenAllocWriteProfileAThenReturnedBufferIsExactSize()
    {
        var target = new ProfileTarget { Id = 1, Code = "A", Active = true, Label = "B" };
        var buffer = ProfileMappers.WriteAAlloc(target);

        Assert.Equal(20, buffer.Length);
    }

    [Fact]
    public void WhenAllocWriteProfileAThenBytesMatchSpanWriter()
    {
        var target = new ProfileTarget { Id = 42, Code = "CODE", Active = false, Label = "LBL" };
        var spanBuffer = new byte[20];
        ProfileMappers.WriteA(spanBuffer, target);

        var allocBuffer = ProfileMappers.WriteAAlloc(target);

        Assert.Equal(spanBuffer, allocBuffer);
    }
}

public class ProfileAMapperReadTests
{
    [Fact]
    public void WhenReadProfileAThenIdIsCorrect()
    {
        var target = new ProfileTarget { Id = 55, Code = "READ", Active = true, Label = "AAA" };
        var buffer = ProfileMappers.WriteAAlloc(target);
        var result = new ProfileTarget();
        ProfileMappers.ReadA(buffer, result);

        Assert.Equal(55, result.Id);
    }

    [Fact]
    public void WhenReadProfileAThenCodeIsTrimmed()
    {
        var target = new ProfileTarget { Id = 1, Code = "READ", Active = true, Label = "X" };
        var buffer = ProfileMappers.WriteAAlloc(target);
        var result = new ProfileTarget();
        ProfileMappers.ReadA(buffer, result);

        Assert.Equal("READ", result.Code);
    }

    [Fact]
    public void WhenReadProfileAThenActiveIsCorrect()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "Y" };
        var buffer = ProfileMappers.WriteAAlloc(target);
        var result = new ProfileTarget();
        ProfileMappers.ReadA(buffer, result);

        Assert.Equal(true, result.Active);
    }

    [Fact]
    public void WhenReadProfileAThenLabelIsTrimmed()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "TAG" };
        var buffer = ProfileMappers.WriteAAlloc(target);
        var result = new ProfileTarget();
        ProfileMappers.ReadA(buffer, result);

        Assert.Equal("TAG", result.Label);
    }

    [Fact]
    public void WhenNewInstanceReadProfileAThenReturnsNewObject()
    {
        var target = new ProfileTarget { Id = 7, Code = "NEW", Active = false, Label = "NI" };
        var buffer = ProfileMappers.WriteAAlloc(target);
        var result = ProfileMappers.ReadANew(buffer);

        Assert.NotNull(result);
    }
}

public class ProfileARoundTripTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WhenRoundTripProfileAThenActiveIsPreserved(bool? active)
    {
        var original = new ProfileTarget { Id = 1, Code = "ABC", Active = active, Label = "XYZ" };
        var buffer = ProfileMappers.WriteAAlloc(original);
        var read = ProfileMappers.ReadANew(buffer);

        Assert.Equal(active, read.Active);
    }

    [Fact]
    public void WhenRoundTripProfileAThenAllFieldsArePreserved()
    {
        var original = new ProfileTarget { Id = 9999, Code = "ABCDEFGH", Active = true, Label = "LABLBL" };
        var buffer = ProfileMappers.WriteAAlloc(original);
        var read = ProfileMappers.ReadANew(buffer);

        Assert.Equal(original.Id, read.Id);
        Assert.Equal(original.Code, read.Code);
        Assert.Equal(original.Active, read.Active);
        Assert.Equal(original.Label, read.Label);
    }
}

// ---- Tests: Profile B ----

public class ProfileBMapperWriteTests
{
    [Fact]
    public void WhenWriteProfileBThenBufferIs40Bytes()
    {
        var target = new ProfileTarget { Id = 1, Code = "B", Active = true, Label = "L" };
        var buffer = ProfileMappers.WriteBAlloc(target);

        Assert.Equal(40, buffer.Length);
    }

    [Fact]
    public void WhenWriteProfileBThenIdIsBigEndianAtOffset0()
    {
        var target = new ProfileTarget { Id = 256, Code = "B", Active = true, Label = "L" };
        var buffer = ProfileMappers.WriteBAlloc(target);

        Assert.Equal(new byte[] { 0x00, 0x00, 0x01, 0x00 }, buffer[..4]);
    }

    [Fact]
    public void WhenWriteProfileBThenCodeHasWiderField()
    {
        var target = new ProfileTarget { Id = 1, Code = "LONGCODE12345678", Active = false, Label = "L" };
        var buffer = ProfileMappers.WriteBAlloc(target);

        // Code field is 16 bytes wide
        Assert.Equal("LONGCODE12345678", Encoding.ASCII.GetString(buffer[4..20]));
    }

    [Fact]
    public void WhenWriteProfileBThenActiveIsAtOffset20()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "L" };
        var buffer = ProfileMappers.WriteBAlloc(target);

        Assert.Equal(0x31, buffer[20]);
    }
}

public class ProfileBRoundTripTests
{
    [Fact]
    public void WhenRoundTripProfileBThenAllFieldsArePreserved()
    {
        var original = new ProfileTarget { Id = 12345, Code = "CODE1234", Active = false, Label = "LabelText123456789" };
        var buffer = ProfileMappers.WriteBAlloc(original);
        var read = ProfileMappers.ReadBNew(buffer);

        Assert.Equal(original.Id, read.Id);
        Assert.Equal(original.Code, read.Code);
        Assert.Equal(original.Active, read.Active);
        Assert.Equal(original.Label, read.Label);
    }
}

// ---- Tests: Profile A vs Profile B produce different layouts ----

public class ProfileLayoutDifferenceTests
{
    [Fact]
    public void WhenSameTargetWrittenWithDifferentProfilesThenBufferSizesDiffer()
    {
        var target = new ProfileTarget { Id = 1, Code = "X", Active = true, Label = "Y" };

        var bufferA = ProfileMappers.WriteAAlloc(target);
        var bufferB = ProfileMappers.WriteBAlloc(target);

        Assert.NotEqual(bufferA.Length, bufferB.Length);
    }

    [Fact]
    public void WhenSameTargetWrittenWithDifferentProfilesThenByteRepresentationsDiffer()
    {
        var target = new ProfileTarget { Id = 1, Code = "SAME", Active = true, Label = "SAME" };

        var bufferA = ProfileMappers.WriteAAlloc(target);
        var bufferB = ProfileMappers.WriteBAlloc(target);

        // Active flag is at different offsets, so raw bytes differ
        Assert.NotEqual(bufferA, bufferB[..bufferA.Length]);
    }

    [Fact]
    public void WhenBufferReadWithWrongProfileThenResultDiffersFromOriginal()
    {
        // Write with Profile A, then attempt to read with Profile B
        // The layouts differ so the values will not match
        var original = new ProfileTarget { Id = 0x41424344, Code = "CODE", Active = true, Label = "LBL" };
        var bufferA = ProfileMappers.WriteAAlloc(original);

        // Pad bufferA to Profile B size
        var paddedBuffer = new byte[40];
        bufferA.CopyTo(paddedBuffer, 0);

        var readWithB = ProfileMappers.ReadBNew(paddedBuffer);

        // Active flag is at different offsets; results should differ
        Assert.NotEqual(original.Active, readWithB.Active);
    }
}
