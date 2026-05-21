namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Options.Converters;

// ---- カスタムコンバーター定義（テスト内で定義） ----

/// <summary>
/// テスト用のカスタムコンバーター：固定長バイト配列を16進数文字列として読み書きします。
/// </summary>
internal sealed class HexStringConverter
{
    public int Size { get; }

    public HexStringConverter(int byteCount)
    {
        Size = byteCount;
    }

    public string Read(ReadOnlySpan<byte> buffer)
    {
        return Convert.ToHexString(buffer[..Size]);
    }

    public void Write(Span<byte> buffer, string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            buffer[..Size].Clear();
            return;
        }

        var hex = value.Length >= (Size * 2) ? value[..(Size * 2)] : value.PadLeft(Size * 2, '0');
        var bytes = Convert.FromHexString(hex);
        bytes.CopyTo(buffer[..Size]);
    }
}

// ---- テスト用レコード ----

public sealed record AsciiRecord(string Code, string Name);

public sealed record IntegerRecord(int? Count, long? Amount);

public sealed record DecimalRecord(decimal? Price);

public sealed record DateTimeRecord(DateTime? EventAt);

public sealed record UnicodeRecord(string Title);

public sealed record CustomConverterRecord(string HexCode, string Label);

// ---- オプションコンバーターのテスト ----

public class OptionsConverterTests
{
    //--------------------------------------------------------------------------------
    // AsciiConverter
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenAsciiReadThenTrimsSpacePadding()
    {
        var converter = new AsciiConverter(10);
        var buffer = "Hello     "u8.ToArray().AsSpan();
        Assert.Equal("Hello", converter.Read(buffer));
    }

    [Fact]
    public void WhenAsciiWriteRightPaddingThenFillsSpace()
    {
        var converter = new AsciiConverter(10, padding: Padding.Right);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, "Hi");
        Assert.Equal("Hi        ", System.Text.Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenAsciiWriteNullThenFillsWithFiller()
    {
        var converter = new AsciiConverter(5);
        Span<byte> buffer = stackalloc byte[5];
        converter.Write(buffer, null);
        Assert.Equal("     ", System.Text.Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenAsciiRoundTripThenValuePreserved()
    {
        var converter = new AsciiConverter(8);
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, "ABCD");
        Assert.Equal("ABCD", converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // IntegerConverter
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenIntegerReadValidValueThenReturnsInt()
    {
        var converter = new IntegerConverter<int>(6);
        var buffer = "   123"u8.ToArray().AsSpan();
        Assert.Equal(123, converter.Read(buffer));
    }

    [Fact]
    public void WhenIntegerReadEmptyThenReturnsNull()
    {
        var converter = new IntegerConverter<int>(4);
        var buffer = "    "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenIntegerWriteZerofillThenPadsWithZero()
    {
        var converter = new IntegerConverter<int>(6, zerofill: true);
        Span<byte> buffer = stackalloc byte[6];
        converter.Write(buffer, 42);
        Assert.Equal("000042", System.Text.Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenIntegerRoundTripNegativeThenValuePreserved()
    {
        var converter = new IntegerConverter<int>(8);
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, -9876);
        Assert.Equal(-9876, converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // DecimalConverter
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenDecimalReadValidValueThenReturnsDecimal()
    {
        var converter = new DecimalConverter(10);
        var buffer = "    123.45"u8.ToArray().AsSpan();
        Assert.Equal(123.45m, converter.Read(buffer));
    }

    [Fact]
    public void WhenDecimalReadEmptyThenReturnsNull()
    {
        var converter = new DecimalConverter(8);
        var buffer = "        "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenDecimalWriteWithScaleThenFormatsCorrectly()
    {
        var converter = new DecimalConverter(10, scale: 2, zerofill: true);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, 12.3m);
        Assert.Equal("0000012.30", System.Text.Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenDecimalRoundTripThenValuePreserved()
    {
        var converter = new DecimalConverter(12, scale: 3);
        Span<byte> buffer = stackalloc byte[12];
        converter.Write(buffer, 999.123m);
        Assert.Equal(999.123m, converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // DateTimeConverter
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenDateTimeReadValidDateThenReturnsDateTime()
    {
        var converter = new DateTimeConverter("yyyyMMdd");
        var buffer = "20240315"u8.ToArray().AsSpan();
        var result = converter.Read(buffer);
        Assert.Equal(new DateTime(2024, 3, 15), result);
    }

    [Fact]
    public void WhenDateTimeReadEmptyThenReturnsNull()
    {
        var converter = new DateTimeConverter("yyyyMMdd");
        var buffer = "        "u8.ToArray().AsSpan();
        Assert.Null(converter.Read(buffer));
    }

    [Fact]
    public void WhenDateTimeWriteNullThenFillsFiller()
    {
        var converter = new DateTimeConverter("yyyyMMdd");
        Span<byte> buffer = stackalloc byte[8];
        converter.Write(buffer, null);
        Assert.Equal("        ", System.Text.Encoding.ASCII.GetString(buffer));
    }

    [Fact]
    public void WhenDateTimeRoundTripThenValuePreserved()
    {
        var converter = new DateTimeConverter("yyyyMMddHHmmss");
        var dt = new DateTime(2024, 6, 1, 12, 30, 59);
        Span<byte> buffer = stackalloc byte[14];
        converter.Write(buffer, dt);
        Assert.Equal(dt, converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // UnicodeConverter
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenUnicodeReadThenTrimsSpacePadding()
    {
        var converter = new UnicodeConverter(20);
        // 10文字 + 5文字分スペース（各2バイト）= 20バイト
        Span<byte> buffer = stackalloc byte[20];
        converter.Write(buffer, "こんにちは");
        Assert.Equal("こんにちは", converter.Read(buffer));
    }

    [Fact]
    public void WhenUnicodeWriteNullThenFillsFiller()
    {
        var converter = new UnicodeConverter(10);
        Span<byte> buffer = stackalloc byte[10];
        converter.Write(buffer, null);
        // すべてスペース文字（0x0020 LE）
        for (var i = 0; i < 10; i += 2)
        {
            Assert.Equal(0x20, buffer[i]);
            Assert.Equal(0x00, buffer[i + 1]);
        }
    }

    [Fact]
    public void WhenUnicodeRoundTripThenValuePreserved()
    {
        var converter = new UnicodeConverter(20);
        Span<byte> buffer = stackalloc byte[20];
        converter.Write(buffer, "テスト");
        Assert.Equal("テスト", converter.Read(buffer));
    }

    //--------------------------------------------------------------------------------
    // カスタムコンバーター（テスト側で定義した HexStringConverter）
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenCustomHexConverterReadThenReturnsHexString()
    {
        // 4バイトのバイナリデータ → "48454C4C" ("HELL" のASCIIコード)
        var converter = new HexStringConverter(4);
        var buffer = new byte[] { 0x48, 0x45, 0x4C, 0x4C };
        Assert.Equal("48454C4C", converter.Read(buffer));
    }

    [Fact]
    public void WhenCustomHexConverterWriteThenFillsAsciiHex()
    {
        var converter = new HexStringConverter(2);
        Span<byte> buffer = stackalloc byte[2];
        converter.Write(buffer, "4142");
        Assert.Equal(0x41, buffer[0]);
        Assert.Equal(0x42, buffer[1]);
    }

    [Fact]
    public void WhenCustomHexConverterWriteNullThenFillsZeros()
    {
        var converter = new HexStringConverter(3);
        Span<byte> buffer = stackalloc byte[3];
        converter.Write(buffer, null);
        Assert.All(buffer.ToArray(), b => Assert.Equal((byte)0x00, b));
    }

    [Fact]
    public void WhenCustomHexConverterRoundTripThenValuePreserved()
    {
        var converter = new HexStringConverter(2);
        Span<byte> buffer = stackalloc byte[2];
        converter.Write(buffer, "FFAA");
        Assert.Equal("FFAA", converter.Read(buffer));
    }
}
