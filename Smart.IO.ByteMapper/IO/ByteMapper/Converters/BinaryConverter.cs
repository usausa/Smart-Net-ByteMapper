namespace Smart.IO.ByteMapper.Converters;

using System.Buffers.Binary;

using Smart.IO.ByteMapper.Helpers;

//--------------------------------------------------------------------------------
// Integer
//--------------------------------------------------------------------------------

internal sealed class BigEndianIntBinaryConverter : IMapConverter
{
    public static BigEndianIntBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt32BigEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)value);
}

internal sealed class LittleEndianIntBinaryConverter : IMapConverter
{
    public static LittleEndianIntBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt32LittleEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt32LittleEndian(buffer, (int)value);
}

//--------------------------------------------------------------------------------
// Long
//--------------------------------------------------------------------------------

internal sealed class BigEndianLongBinaryConverter : IMapConverter
{
    public static BigEndianLongBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt64BigEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt64BigEndian(buffer, (long)value);
}

internal sealed class LittleEndianLongBinaryConverter : IMapConverter
{
    public static LittleEndianLongBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt64LittleEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt64LittleEndian(buffer, (long)value);
}

//--------------------------------------------------------------------------------
// Short
//--------------------------------------------------------------------------------

internal sealed class BigEndianShortBinaryConverter : IMapConverter
{
    public static BigEndianShortBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt16BigEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)value);
}

internal sealed class LittleEndianShortBinaryConverter : IMapConverter
{
    public static LittleEndianShortBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BinaryPrimitives.ReadInt16LittleEndian(buffer);

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt16LittleEndian(buffer, (short)value);
}

//--------------------------------------------------------------------------------
// Double
//--------------------------------------------------------------------------------

internal sealed class BigEndianDoubleBinaryConverter : IMapConverter
{
    public static BigEndianDoubleBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BytesHelper.Int64ToDouble(BinaryPrimitives.ReadInt64BigEndian(buffer));

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt64BigEndian(buffer, BytesHelper.DoubleToInt64((double)value));
}

internal sealed class LittleEndianDoubleBinaryConverter : IMapConverter
{
    public static LittleEndianDoubleBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BytesHelper.Int64ToDouble(BinaryPrimitives.ReadInt64LittleEndian(buffer));

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt64LittleEndian(buffer, BytesHelper.DoubleToInt64((double)value));
}

//--------------------------------------------------------------------------------
// Float
//--------------------------------------------------------------------------------

internal sealed class BigEndianFloatBinaryConverter : IMapConverter
{
    public static BigEndianFloatBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BytesHelper.Int32ToFloat(BinaryPrimitives.ReadInt32BigEndian(buffer));

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt32BigEndian(buffer, BytesHelper.FloatToInt32((float)value));
}

internal sealed class LittleEndianFloatBinaryConverter : IMapConverter
{
    public static LittleEndianFloatBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer) =>
        BytesHelper.Int32ToFloat(BinaryPrimitives.ReadInt32LittleEndian(buffer));

    public void Write(Span<byte> buffer, object value) =>
        BinaryPrimitives.WriteInt32LittleEndian(buffer, BytesHelper.FloatToInt32((float)value));
}

//--------------------------------------------------------------------------------
// Decimal
//--------------------------------------------------------------------------------

internal sealed class BigEndianDecimalBinaryConverter : IMapConverter
{
    public static BigEndianDecimalBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var span = buffer;
        var flag = BinaryPrimitives.ReadInt32BigEndian(span);
        var hi = BinaryPrimitives.ReadInt32BigEndian(span[4..]);
        var mid = BinaryPrimitives.ReadInt32BigEndian(span[8..]);
        var lo = BinaryPrimitives.ReadInt32BigEndian(span[12..]);
        return DecimalHelper.FromBits(lo, mid, hi, flag);
    }

    public void Write(Span<byte> buffer, object value)
    {
        var span = buffer;
        var bits = Decimal.GetBits((decimal)value);
        BinaryPrimitives.WriteInt32BigEndian(span, bits[3]);
        BinaryPrimitives.WriteInt32BigEndian(span[4..], bits[2]);
        BinaryPrimitives.WriteInt32BigEndian(span[8..], bits[1]);
        BinaryPrimitives.WriteInt32BigEndian(span[12..], bits[0]);
    }
}

internal sealed class LittleEndianDecimalBinaryConverter : IMapConverter
{
    public static LittleEndianDecimalBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var span = buffer;
        var lo = BinaryPrimitives.ReadInt32LittleEndian(span);
        var mid = BinaryPrimitives.ReadInt32LittleEndian(span[4..]);
        var hi = BinaryPrimitives.ReadInt32LittleEndian(span[8..]);
        var flag = BinaryPrimitives.ReadInt32LittleEndian(span[12..]);
        return DecimalHelper.FromBits(lo, mid, hi, flag);
    }

    public void Write(Span<byte> buffer, object value)
    {
        var span = buffer;
        var bits = Decimal.GetBits((decimal)value);
        BinaryPrimitives.WriteInt32LittleEndian(span, bits[0]);
        BinaryPrimitives.WriteInt32LittleEndian(span[4..], bits[1]);
        BinaryPrimitives.WriteInt32LittleEndian(span[8..], bits[2]);
        BinaryPrimitives.WriteInt32LittleEndian(span[12..], bits[3]);
    }
}

//--------------------------------------------------------------------------------
// DateTime
//--------------------------------------------------------------------------------

internal sealed class BigEndianDateTimeBinaryConverter : IMapConverter
{
    private readonly DateTimeKind kind;

    public BigEndianDateTimeBinaryConverter(DateTimeKind kind)
    {
        this.kind = kind;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var ticks = BinaryPrimitives.ReadInt64BigEndian(buffer);
        return DateTimeHelper.IsValidTicks(ticks)
            ? new DateTime(ticks, kind)
            : default;
    }

    public void Write(Span<byte> buffer, object value)
    {
        BinaryPrimitives.WriteInt64BigEndian(buffer, ((DateTime)value).Ticks);
    }
}

internal sealed class LittleEndianDateTimeBinaryConverter : IMapConverter
{
    private readonly DateTimeKind kind;

    public LittleEndianDateTimeBinaryConverter(DateTimeKind kind)
    {
        this.kind = kind;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var ticks = BinaryPrimitives.ReadInt64LittleEndian(buffer);
        return DateTimeHelper.IsValidTicks(ticks)
            ? new DateTime(ticks, kind)
            : default;
    }

    public void Write(Span<byte> buffer, object value)
    {
        BinaryPrimitives.WriteInt64LittleEndian(buffer, ((DateTime)value).Ticks);
    }
}

//--------------------------------------------------------------------------------
// DateTimeOffset
//--------------------------------------------------------------------------------

internal sealed class BigEndianDateTimeOffsetBinaryConverter : IMapConverter
{
    public static BigEndianDateTimeOffsetBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var span = buffer;
        var ticks = BinaryPrimitives.ReadInt64BigEndian(span);
        var offset = BinaryPrimitives.ReadInt16BigEndian(span[8..]);
        return DateTimeHelper.IsValidTicks(ticks) && DateTimeHelper.IsValidOffset(offset)
            ? new DateTimeOffset(new DateTime(ticks, DateTimeKind.Unspecified), TimeSpan.FromMinutes(offset))
            : default;
    }

    public void Write(Span<byte> buffer, object value)
    {
        var span = buffer;
        var dateTime = (DateTimeOffset)value;
        BinaryPrimitives.WriteInt64BigEndian(span, dateTime.UtcTicks);
        BinaryPrimitives.WriteInt16BigEndian(span[8..], (short)(dateTime.Offset.Ticks / TimeSpan.TicksPerMinute));
    }
}

internal sealed class LittleEndianDateTimeOffsetBinaryConverter : IMapConverter
{
    public static LittleEndianDateTimeOffsetBinaryConverter Default { get; } = new();

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var span = buffer;
        var ticks = BinaryPrimitives.ReadInt64LittleEndian(span);
        var offset = BinaryPrimitives.ReadInt16LittleEndian(span[8..]);
        return DateTimeHelper.IsValidTicks(ticks) && DateTimeHelper.IsValidOffset(offset)
            ? new DateTimeOffset(new DateTime(ticks, DateTimeKind.Unspecified), TimeSpan.FromMinutes(offset))
            : default;
    }

    public void Write(Span<byte> buffer, object value)
    {
        var span = buffer;
        var dateTime = (DateTimeOffset)value;
        BinaryPrimitives.WriteInt64LittleEndian(span, dateTime.UtcTicks);
        BinaryPrimitives.WriteInt16LittleEndian(span[8..], (short)(dateTime.Offset.Ticks / TimeSpan.TicksPerMinute));
    }
}
