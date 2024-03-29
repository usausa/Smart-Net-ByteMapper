namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

public sealed class BigEndianDateTimeBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly DateTime Value = new(1);

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01]);

    private readonly BigEndianDateTimeBinaryConverter converter = new(DateTimeKind.Local);

    [Fact]
    public void ReadToBigEndianDateTimeBinary()
    {
        Assert.Equal(Value, (DateTime)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void ReadToBigEndianInvalidDateTimeIsDefault()
    {
        Assert.Equal(default, (DateTime)converter.Read([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF], 0));
    }

    [Fact]
    public void WriteBigEndianDateTimeBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianDateTimeBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly DateTime Value = new(1);

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);

    private readonly LittleEndianDateTimeBinaryConverter converter = new(DateTimeKind.Local);

    [Fact]
    public void ReadToLittleEndianDateTimeBinary()
    {
        Assert.Equal(Value, (DateTime)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void ReadToLittleEndianInvalidDateTimeIsDefault()
    {
        Assert.Equal(default, (DateTime)converter.Read([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF], 0));
    }

    [Fact]
    public void WriteLittleEndianDateTimeBinaryToBuffer()
    {
        var buffer = new byte[8 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class BigEndianDateTimeOffsetBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly DateTimeOffset Value = new(new DateTime(1, DateTimeKind.Utc));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00]);

    private readonly BigEndianDateTimeOffsetBinaryConverter converter = BigEndianDateTimeOffsetBinaryConverter.Default;

    [Fact]
    public void ReadToBigEndianDateTimeOffsetBinary()
    {
        Assert.Equal(Value, (DateTimeOffset)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void ReadToBigEndianInvalidDateTimeOffsetIsDefault()
    {
        Assert.Equal(default, (DateTimeOffset)converter.Read([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00], 0));
        Assert.Equal(default, (DateTimeOffset)converter.Read([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xFF], 0));
    }

    [Fact]
    public void WriteBigEndianDateTimeOffsetBinaryToBuffer()
    {
        var buffer = new byte[10 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}

public sealed class LittleEndianDateTimeOffsetBinaryConverterTest
{
    private const int Offset = 1;

    private static readonly DateTimeOffset Value = new(new DateTime(1, DateTimeKind.Utc));

    private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, [0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);

    private readonly LittleEndianDateTimeOffsetBinaryConverter converter = LittleEndianDateTimeOffsetBinaryConverter.Default;

    [Fact]
    public void ReadToLittleEndianDateTimeOffsetBinary()
    {
        Assert.Equal(Value, (DateTimeOffset)converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void ReadToLittleEndianInvalidDateTimeOffsetIsDefault()
    {
        Assert.Equal(default, (DateTimeOffset)converter.Read([0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00], 0));
        Assert.Equal(default, (DateTimeOffset)converter.Read([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F], 0));
    }

    [Fact]
    public void WriteLittleEndianDateTimeOffsetBinaryToBuffer()
    {
        var buffer = new byte[10 + Offset];
        converter.Write(buffer, Offset, Value);

        Assert.Equal(ValueBytes, buffer);
    }
}
