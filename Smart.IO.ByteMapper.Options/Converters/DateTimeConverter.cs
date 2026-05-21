namespace Smart.IO.ByteMapper.Options.Converters;

using Smart.IO.ByteMapper.Options.Helpers;

/// <summary>ASCII バイト表現の DateTime/DateTimeOffset コンバーター。</summary>
public sealed class DateTimeConverter
{
    private readonly bool hasDatePart;
    private readonly DateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="DateTimeConverter"/> の新しいインスタンスを初期化します。</summary>
    public DateTimeConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        Size = format.Length;
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    /// <summary>バッファーから DateTime を読み取ります。</summary>
    public DateTime? Read(ReadOnlySpan<byte> buffer)
    {
        return DateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result) ? result : null;
    }

    /// <summary>バッファーへ DateTime を書き込みます。</summary>
    public void Write(Span<byte> buffer, DateTime? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value);
        }
    }
}

/// <summary>ASCII バイト表現の DateTimeOffset コンバーター。</summary>
public sealed class DateTimeOffsetConverter
{
    private readonly bool hasDatePart;
    private readonly DateTimeFormatEntry[] entries;
    private readonly DateTimeKind kind;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="DateTimeOffsetConverter"/> の新しいインスタンスを初期化します。</summary>
    public DateTimeOffsetConverter(string format, DateTimeKind kind = DateTimeKind.Unspecified, byte filler = 0x20)
    {
        Size = format.Length;
        entries = DateTimeByteHelper.ParseDateTimeFormat(format, out hasDatePart);
        this.kind = kind;
        this.filler = filler;
    }

    /// <summary>バッファーから DateTimeOffset を読み取ります。</summary>
    public DateTimeOffset? Read(ReadOnlySpan<byte> buffer)
    {
        if (!DateTimeByteHelper.TryParseDateTime(buffer, 0, entries, kind, out var result))
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

    /// <summary>バッファーへ DateTimeOffset を書き込みます。</summary>
    public void Write(Span<byte> buffer, DateTimeOffset? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
        }
        else
        {
            DateTimeByteHelper.FormatDateTime(buffer, 0, hasDatePart, entries, value.Value.UtcDateTime);
        }
    }
}
