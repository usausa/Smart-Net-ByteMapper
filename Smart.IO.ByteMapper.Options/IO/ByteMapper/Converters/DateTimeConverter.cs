namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class DateTimeConverter : IMapConverter
{
    private readonly int length;

    private readonly bool hasDatePart;

    private readonly DateTimeFormatEntry[] entries;

    private readonly DateTimeKind kind;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateTimeConverter(string format, DateTimeKind kind, byte filler, Type type)
    {
        length = format.Length;
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return DateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result)
            ? result
            : defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, (DateTime)value);
        }
    }
}

internal sealed class DateTimeOffsetConverter : IMapConverter
{
    private readonly int length;

    private readonly bool hasDatePart;

    private readonly DateTimeFormatEntry[] entries;

    private readonly DateTimeKind kind;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateTimeOffsetConverter(string format, DateTimeKind kind, byte filler, Type type)
    {
        length = format.Length;
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (DateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result))
        {
            if (kind == DateTimeKind.Unspecified)
            {
                return new DateTimeOffset(result, TimeSpan.Zero);
            }

            if (kind == DateTimeKind.Utc)
            {
                return new DateTimeOffset(result);
            }

            var offset = TimeZoneInfo.Local.GetUtcOffset(result);
            var utcTick = result.Ticks - offset.Ticks;
            if (DateTimeHelper.IsValidTicks(utcTick))
            {
                return new DateTimeOffset(result, offset);
            }
        }

        return defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, ((DateTimeOffset)value).UtcDateTime);
        }
    }
}
