namespace Smart.IO.ByteMapper;

using System;
using System.Globalization;
using System.Text;

// ---- Nullable numbers: null ⇔ all-filler field ----
// Layout: Short(0,4) + Int(4,6) + Long(10,8) + Float(18,8) + Double(26,8) + Decimal(34,10) = 44 bytes

[Map(44, UseDelimiter = false)]
internal sealed class NullableNumberRecord
{
    [MapNumberText<short>(0, 4)]
    public short? ShortValue { get; set; }

    [MapNumberText<int>(4, 6)]
    public int? IntValue { get; set; }

    [MapNumberText<long>(10, 8)]
    public long? LongValue { get; set; }

    [MapNumberText<float>(18, 8, Style = NumberStyles.Float)]
    public float? FloatValue { get; set; }

    [MapNumberText<double>(26, 8, Style = NumberStyles.Float)]
    public double? DoubleValue { get; set; }

    [MapNumberText<decimal>(34, 10, Style = NumberStyles.Number)]
    public decimal? DecimalValue { get; set; }
}

// ---- Non-nullable: all-filler reads back as default ----

[Map(6, UseDelimiter = false)]
internal sealed class StrictNumberRecord
{
    [MapNumberText<int>(0, 6)]
    public int Qty { get; set; }
}

internal static partial class NumberTextMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, NullableNumberRecord target);

    [ByteWriter]
    public static partial void Write(Span<byte> buffer, NullableNumberRecord source);

    [ByteReader]
    public static partial void ReadStrict(ReadOnlySpan<byte> buffer, StrictNumberRecord target);

    [ByteWriter]
    public static partial void WriteStrict(Span<byte> buffer, StrictNumberRecord source);
}

public class NumberTextMapperTests
{
    [Fact]
    public void WhenWriteNullsThenFieldsAreAllFiller()
    {
        var buffer = new byte[44];
        NumberTextMappers.Write(buffer, new NullableNumberRecord());

        Assert.Equal(new string(' ', 44), Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenReadAllFillerThenValuesAreNull()
    {
        var buffer = new byte[44];
        buffer.AsSpan().Fill(0x20);
        var record = new NullableNumberRecord
        {
            ShortValue = 1,
            IntValue = 2,
            LongValue = 3,
            FloatValue = 4f,
            DoubleValue = 5d,
            DecimalValue = 6m
        };
        NumberTextMappers.Read(buffer, record);

        Assert.Null(record.ShortValue);
        Assert.Null(record.IntValue);
        Assert.Null(record.LongValue);
        Assert.Null(record.FloatValue);
        Assert.Null(record.DoubleValue);
        Assert.Null(record.DecimalValue);
    }

    [Fact]
    public void WhenRoundTripValuesThenValuesArePreserved()
    {
        var original = new NullableNumberRecord
        {
            ShortValue = 12,
            IntValue = -345,
            LongValue = 6789,
            FloatValue = 1.5f,
            DoubleValue = -2.5d,
            DecimalValue = 1234.56m
        };
        var buffer = new byte[44];
        NumberTextMappers.Write(buffer, original);

        var restored = new NullableNumberRecord();
        NumberTextMappers.Read(buffer, restored);

        Assert.Equal(original.ShortValue, restored.ShortValue);
        Assert.Equal(original.IntValue, restored.IntValue);
        Assert.Equal(original.LongValue, restored.LongValue);
        Assert.Equal(original.FloatValue, restored.FloatValue);
        Assert.Equal(original.DoubleValue, restored.DoubleValue);
        Assert.Equal(original.DecimalValue, restored.DecimalValue);
    }

    [Fact]
    public void WhenRoundTripMixedNullThenEachFieldIsIndependent()
    {
        var original = new NullableNumberRecord
        {
            ShortValue = null,
            IntValue = 123,
            LongValue = null,
            FloatValue = null,
            DoubleValue = null,
            DecimalValue = 99.5m
        };
        var buffer = new byte[44];
        NumberTextMappers.Write(buffer, original);

        // Short(4,null) + Int(6,"   123") + Long/Float/Double(24,null) + Decimal(10,"      99.5")
        Assert.Equal("       123" + new string(' ', 24) + "      99.5", Encoding.ASCII.GetString(buffer));

        var restored = new NullableNumberRecord { ShortValue = 7, LongValue = 8 };
        NumberTextMappers.Read(buffer, restored);

        Assert.Null(restored.ShortValue);
        Assert.Equal(123, restored.IntValue);
        Assert.Null(restored.LongValue);
        Assert.Null(restored.FloatValue);
        Assert.Null(restored.DoubleValue);
        Assert.Equal(99.5m, restored.DecimalValue);
    }

    [Fact]
    public void WhenStrictReadAllFillerThenValueIsDefault()
    {
        // 非 nullable プロパティは .GetValueOrDefault() 経由で default (0) になる
        var buffer = "      "u8.ToArray();
        var record = new StrictNumberRecord { Qty = 999 };
        NumberTextMappers.ReadStrict(buffer, record);

        Assert.Equal(0, record.Qty);
    }

    [Fact]
    public void WhenStrictRoundTripThenValueIsPreserved()
    {
        var original = new StrictNumberRecord { Qty = -123 };
        var buffer = new byte[6];
        NumberTextMappers.WriteStrict(buffer, original);

        Assert.Equal("  -123", Encoding.ASCII.GetString(buffer));

        var restored = new StrictNumberRecord();
        NumberTextMappers.ReadStrict(buffer, restored);

        Assert.Equal(original.Qty, restored.Qty);
    }
}
