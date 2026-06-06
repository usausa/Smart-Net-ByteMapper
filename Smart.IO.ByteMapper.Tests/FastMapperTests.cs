namespace Smart.IO.ByteMapper;

using System;

// Fast converter attributes exercised through the generator in both forms:
//   * property form ([Map] on the entity, [MapFast...] on its properties)
//   * member form ([MapProfile] profile, [MapFast...Member] class attributes)
// The Fast numeric/date-time converters use nullable property types.

// ---- Entity self-mapped with the Fast property attributes (40 bytes) ----
// Id(0,10) + Amount(10,12) + Date(22,8) + Name(30,10)

[Map(40, UseDelimiter = false)]
internal sealed class FastEntity
{
    [MapFastInteger<int>(0, 10)]
    public int? Id { get; set; }

    [MapFastDecimal(10, 12, Scale = 2)]
    public decimal? Amount { get; set; }

    [MapFastDateTime(22, "yyyyMMdd")]
    public DateTime? Date { get; set; }

    [MapFastUnicode(30, 10)]
    public string Name { get; set; } = default!;
}

// ---- Member profile describing the same layout for the same entity ----

#pragma warning disable CA1812
[MapProfile(40, UseDelimiter = false)]
[MapFastIntegerMember<int>(nameof(FastEntity.Id), 0, 10)]
[MapFastDecimalMember(nameof(FastEntity.Amount), 10, 12, Scale = 2)]
[MapFastDateTimeMember(nameof(FastEntity.Date), 22, "yyyyMMdd")]
[MapFastUnicodeMember(nameof(FastEntity.Name), 30, 10)]
internal sealed class FastMemberProfile
{
}
#pragma warning restore CA1812

// ---- DateTimeOffset coverage (8 bytes) ----

[Map(8, UseDelimiter = false)]
internal sealed class FastOffsetEntity
{
    [MapFastDateTimeOffset(0, "yyyyMMdd")]
    public DateTimeOffset? Stamp { get; set; }
}

#pragma warning disable CA1812
[MapProfile(8, UseDelimiter = false)]
[MapFastDateTimeOffsetMember(nameof(FastOffsetEntity.Stamp), 0, "yyyyMMdd")]
internal sealed class FastOffsetProfile
{
}
#pragma warning restore CA1812

internal static partial class FastMappers
{
    [ByteWriter]
    public static partial void Write(Span<byte> buffer, FastEntity source);

    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, FastEntity target);

    [ByteWriter(Profile = typeof(FastMemberProfile))]
    public static partial void WriteProfile(Span<byte> buffer, FastEntity source);

    [ByteReader(Profile = typeof(FastMemberProfile))]
    public static partial void ReadProfile(ReadOnlySpan<byte> buffer, FastEntity target);

    [ByteWriter]
    public static partial void WriteOffset(Span<byte> buffer, FastOffsetEntity source);

    [ByteWriter(Profile = typeof(FastOffsetProfile))]
    public static partial void WriteOffsetProfile(Span<byte> buffer, FastOffsetEntity source);
}

public class FastMapperTests
{
    [Fact]
    public void WhenMemberProfileWritesThenBytesMatchPropertyForm()
    {
        var entity = new FastEntity { Id = 42, Amount = 12.34m, Date = new DateTime(2026, 6, 6), Name = "Test" };

        var self = new byte[40];
        var profile = new byte[40];
        FastMappers.Write(self, entity);
        FastMappers.WriteProfile(profile, entity);

        Assert.Equal(self, profile);
    }

    [Fact]
    public void WhenMemberProfileRoundTripsThenValuesAreRestored()
    {
        var entity = new FastEntity { Id = 42, Amount = 12.34m, Date = new DateTime(2026, 6, 6), Name = "Test" };

        var buffer = new byte[40];
        FastMappers.WriteProfile(buffer, entity);
        var restored = new FastEntity();
        FastMappers.ReadProfile(buffer, restored);

        Assert.Equal(entity.Id, restored.Id);
        Assert.Equal(entity.Amount, restored.Amount);
        Assert.Equal(entity.Date, restored.Date);
        Assert.Equal(entity.Name, restored.Name);
    }

    [Fact]
    public void WhenFastDateTimeOffsetMemberWritesThenBytesMatchPropertyForm()
    {
        var entity = new FastOffsetEntity { Stamp = new DateTimeOffset(2026, 6, 6, 0, 0, 0, TimeSpan.Zero) };

        var self = new byte[8];
        var profile = new byte[8];
        FastMappers.WriteOffset(self, entity);
        FastMappers.WriteOffsetProfile(profile, entity);

        Assert.Equal(self, profile);
    }
}
