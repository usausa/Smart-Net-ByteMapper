namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

// ASCII バイト表現の DateTime/DateTimeOffset コンバーター。
public sealed class FastDateTimeConverter
{
    private readonly bool hasDatePart;
    private readonly FastDateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    // フィールドのバイト長を取得します。
    public int Size { get; }

    // FastDateTimeConverter の新しいインスタンスを初期化します。
    public FastDateTimeConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        Size = format.Length;
        entries = FastDateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    // バッファーから DateTime を読み取ります。
    public DateTime? Read(ReadOnlySpan<byte> buffer)
    {
        return FastDateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result) ? result : null;
    }

    // バッファーへ DateTime を書き込みます。
    public void Write(Span<byte> buffer, DateTime? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            FastDateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value);
        }
    }
}

// ASCII バイト表現の DateTimeOffset コンバーター。
public sealed class FastDateTimeOffsetConverter
{
    private readonly bool hasDatePart;
    private readonly FastDateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    // フィールドのバイト長を取得します。
    public int Size { get; }

    // FastDateTimeOffsetConverter の新しいインスタンスを初期化します。
    public FastDateTimeOffsetConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        Size = format.Length;
        entries = FastDateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    // バッファーから DateTimeOffset を読み取ります。
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
        if (utcTick < DateTime.MinValue.Ticks || utcTick > DateTime.MaxValue.Ticks)
        {
            return null;
        }

        return new DateTimeOffset(result, offset);
    }

    // バッファーへ DateTimeOffset を書き込みます。
    public void Write(Span<byte> buffer, DateTimeOffset? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            FastDateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value.UtcDateTime);
        }
    }
}
