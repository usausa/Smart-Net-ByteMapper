namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

#pragma warning disable IDE0032
public sealed class FastDateTimeConverter
{
    private readonly int size;
    private readonly bool hasDatePart;
    private readonly FastDateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    public int Size => size;

    public FastDateTimeConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        size = format.Length;
        entries = FastDateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    public DateTime? Read(ReadOnlySpan<byte> buffer)
    {
        return FastDateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result) ? result : null;
    }

    public void Write(Span<byte> buffer, DateTime? value)
    {
        if (value is null)
        {
            buffer[..size].Fill(filler);
        }
        else
        {
            FastDateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value);
        }
    }
}

public sealed class FastDateTimeOffsetConverter
{
    private readonly int size;
    private readonly bool hasDatePart;
    private readonly FastDateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    public int Size => size;

    public FastDateTimeOffsetConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        size = format.Length;
        entries = FastDateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    public DateTimeOffset? Read(ReadOnlySpan<byte> buffer)
    {
        if (!FastDateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result))
        {
            return null;
        }

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
        if ((utcTick < DateTime.MinValue.Ticks) || (utcTick > DateTime.MaxValue.Ticks))
        {
            return null;
        }

        return new DateTimeOffset(result, offset);
    }

    public void Write(Span<byte> buffer, DateTimeOffset? value)
    {
        if (value is null)
        {
            buffer[..size].Fill(filler);
        }
        else
        {
            FastDateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value.UtcDateTime);
        }
    }
}
#pragma warning restore IDE0032
