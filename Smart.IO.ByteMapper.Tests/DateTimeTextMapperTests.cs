namespace Smart.IO.ByteMapper;

using System;
using System.Text;

// ---- Nullable date/time: null ⇔ all-filler field ----
// Layout: Date(0,8) + Time(8,6) = 14 bytes

[Map(14, UseDelimiter = false)]
internal sealed class NullableDateTimeRecord
{
    [MapDateTimeText<DateTime>(0, 8, "yyyyMMdd")]
    public DateTime? Date { get; set; }

    [MapDateTimeText<TimeOnly>(8, 6, "HHmmss")]
    public TimeOnly? Time { get; set; }
}

// ---- Remaining nullable variants: DateTimeOffset? + DateOnly? ----
// Layout: Stamp(0,8) + DateOnlyValue(8,8) = 16 bytes

[Map(16, UseDelimiter = false)]
internal sealed class NullableOffsetDateRecord
{
    [MapDateTimeText<DateTimeOffset>(0, 8, "yyyyMMdd")]
    public DateTimeOffset? Stamp { get; set; }

    [MapDateTimeText<DateOnly>(8, 8, "yyyyMMdd")]
    public DateOnly? DateOnlyValue { get; set; }
}

// ---- Non-nullable: all-filler reads back as default ----

[Map(8, UseDelimiter = false)]
internal sealed class StrictDateTimeRecord
{
    [MapDateTimeText<DateTime>(0, 8, "yyyyMMdd")]
    public DateTime Date { get; set; }
}

internal static partial class DateTimeTextMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, NullableDateTimeRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, NullableDateTimeRecord source);

    [ByteReader]
    public static partial void ReadOffsetDate(ReadOnlySpan<byte> buffer, NullableOffsetDateRecord target);

    [ByteWriter]
    public static partial void WriteOffsetDate(Span<byte> buffer, NullableOffsetDateRecord source);

    [ByteReader]
    public static partial void ReadStrict(ReadOnlySpan<byte> buffer, StrictDateTimeRecord target);

    [ByteWriter]
    public static partial void WriteStrict(Span<byte> buffer, StrictDateTimeRecord source);
}

public class DateTimeTextMapperTests
{
    [Fact]
    public void WhenWriteNullsThenFieldsAreAllFiller()
    {
        var buffer = new byte[14];
        DateTimeTextMappers.Write(buffer, new NullableDateTimeRecord { Date = null, Time = null });

        Assert.Equal("              ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenReadAllFillerThenValuesAreNull()
    {
        var buffer = "              "u8.ToArray();
        var record = new NullableDateTimeRecord { Date = new DateTime(2000, 1, 1), Time = new TimeOnly(1, 2, 3) };
        DateTimeTextMappers.Read(buffer, record);

        Assert.Null(record.Date);
        Assert.Null(record.Time);
    }

    [Fact]
    public void WhenRoundTripValuesThenValuesArePreserved()
    {
        var original = new NullableDateTimeRecord
        {
            Date = new DateTime(2026, 7, 19),
            Time = new TimeOnly(12, 34, 56),
        };
        var buffer = new byte[14];
        DateTimeTextMappers.Write(buffer, original);

        Assert.Equal("20260719123456", Encoding.ASCII.GetString(buffer));

        var restored = new NullableDateTimeRecord();
        DateTimeTextMappers.Read(buffer, restored);

        Assert.Equal(original.Date, restored.Date);
        Assert.Equal(original.Time, restored.Time);
    }

    [Fact]
    public void WhenRoundTripMixedNullThenEachFieldIsIndependent()
    {
        var original = new NullableDateTimeRecord { Date = new DateTime(2026, 7, 19), Time = null };
        var buffer = new byte[14];
        DateTimeTextMappers.Write(buffer, original);

        Assert.Equal("20260719      ", Encoding.ASCII.GetString(buffer));

        var restored = new NullableDateTimeRecord { Time = new TimeOnly(9, 9, 9) };
        DateTimeTextMappers.Read(buffer, restored);

        Assert.Equal(original.Date, restored.Date);
        Assert.Null(restored.Time);
    }

    // ---- DateTimeOffset? / DateOnly? も同じ null ⇔ 全フィラー挙動 ----

    [Fact]
    public void WhenOffsetDateWriteNullsThenFieldsAreAllFiller()
    {
        var buffer = new byte[16];
        DateTimeTextMappers.WriteOffsetDate(buffer, new NullableOffsetDateRecord { Stamp = null, DateOnlyValue = null });

        Assert.Equal("                ", Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenOffsetDateReadAllFillerThenValuesAreNull()
    {
        var buffer = "                "u8.ToArray();
        var record = new NullableOffsetDateRecord
        {
            Stamp = DateTimeOffset.UnixEpoch,
            DateOnlyValue = new DateOnly(2000, 1, 1),
        };
        DateTimeTextMappers.ReadOffsetDate(buffer, record);

        Assert.Null(record.Stamp);
        Assert.Null(record.DateOnlyValue);
    }

    [Fact]
    public void WhenOffsetDateRoundTripValuesThenValuesArePreserved()
    {
        // DateTimeOffset の期待値は読み取りと同じ ParseExact で構築し、マシンのタイムゾーンに依存させない
        var original = new NullableOffsetDateRecord
        {
            Stamp = DateTimeOffset.ParseExact("20260719", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
            DateOnlyValue = new DateOnly(2026, 7, 19),
        };
        var buffer = new byte[16];
        DateTimeTextMappers.WriteOffsetDate(buffer, original);

        Assert.Equal("2026071920260719", Encoding.ASCII.GetString(buffer));

        var restored = new NullableOffsetDateRecord();
        DateTimeTextMappers.ReadOffsetDate(buffer, restored);

        Assert.Equal(original.Stamp, restored.Stamp);
        Assert.Equal(original.DateOnlyValue, restored.DateOnlyValue);
    }

    [Fact]
    public void WhenOffsetDateRoundTripMixedNullThenEachFieldIsIndependent()
    {
        var original = new NullableOffsetDateRecord { Stamp = null, DateOnlyValue = new DateOnly(2026, 7, 19) };
        var buffer = new byte[16];
        DateTimeTextMappers.WriteOffsetDate(buffer, original);

        Assert.Equal("        20260719", Encoding.ASCII.GetString(buffer));

        var restored = new NullableOffsetDateRecord { Stamp = DateTimeOffset.UnixEpoch };
        DateTimeTextMappers.ReadOffsetDate(buffer, restored);

        Assert.Null(restored.Stamp);
        Assert.Equal(original.DateOnlyValue, restored.DateOnlyValue);
    }

    [Fact]
    public void WhenStrictReadAllFillerThenValueIsDefault()
    {
        // 非 nullable プロパティは .GetValueOrDefault() 経由で default になる
        var buffer = "        "u8.ToArray();
        var record = new StrictDateTimeRecord { Date = new DateTime(2000, 1, 1) };
        DateTimeTextMappers.ReadStrict(buffer, record);

        Assert.Equal(default, record.Date);
    }

    [Fact]
    public void WhenStrictRoundTripThenValueIsPreserved()
    {
        var original = new StrictDateTimeRecord { Date = new DateTime(2026, 7, 19) };
        var buffer = new byte[8];
        DateTimeTextMappers.WriteStrict(buffer, original);

        var restored = new StrictDateTimeRecord();
        DateTimeTextMappers.ReadStrict(buffer, restored);

        Assert.Equal(original.Date, restored.Date);
    }
}
