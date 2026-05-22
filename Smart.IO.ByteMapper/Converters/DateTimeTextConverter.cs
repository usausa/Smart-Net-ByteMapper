namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

public sealed class DateTimeTextConverter<T>
    where T : struct
{
    private readonly Encoding encoding;
    private readonly string format;
    private readonly DateTimeStyles style;
    private readonly IFormatProvider provider;
    private readonly byte filler;

    public int Size { get; }

    public DateTimeTextConverter(
        int length,
        string format,
        int codePage = 20127,
        DateTimeStyles style = DateTimeStyles.None,
        Culture culture = Culture.Invariant,
        byte filler = 0x20)
    {
        Size = length;
        this.format = format;
        encoding = ResolveEncoding(codePage);
        this.style = style;
        provider = culture == Culture.Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
        this.filler = filler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(ReadOnlySpan<byte> source)
    {
        // trim filler from right
        var size = Size;
        while ((size > 0) && (source[size - 1] == filler))
        {
            size--;
        }

        if (size == 0)
        {
            return default;
        }

        var str = encoding.GetString(source[..size]);
        return ParseValue(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, T value)
    {
        var str = FormatValue(value);
        if (String.IsNullOrEmpty(str))
        {
            destination[..Size].Fill(filler);
            return;
        }
        Span<byte> encoded = stackalloc byte[encoding.GetMaxByteCount(str.Length)];
        var count = encoding.GetBytes(str, encoded);
        var written = Math.Min(count, Size);
        encoded[..written].CopyTo(destination[..written]);
        if (written < Size)
        {
            destination[written..Size].Fill(filler);
        }
    }

    private T ParseValue(string str)
    {
        if (typeof(T) == typeof(DateTime))
        {
            return Unsafe.BitCast<DateTime, T>(DateTime.ParseExact(str, format, provider, style));
        }

        if (typeof(T) == typeof(DateTimeOffset))
        {
            return Unsafe.BitCast<DateTimeOffset, T>(DateTimeOffset.ParseExact(str, format, provider, style));
        }

        if (typeof(T) == typeof(DateOnly))
        {
            return Unsafe.BitCast<DateOnly, T>(DateOnly.ParseExact(str, format, provider));
        }

        if (typeof(T) == typeof(TimeOnly))
        {
            return Unsafe.BitCast<TimeOnly, T>(TimeOnly.ParseExact(str, format, provider));
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

#pragma warning disable CA1508
    private string FormatValue(T value)
    {
        if (value is IFormattable f)
        {
            return f.ToString(format, provider);
        }

        return value.ToString() ?? string.Empty;
    }
#pragma warning restore CA1508

    private static Encoding ResolveEncoding(int codePage) => codePage switch
    {
        20127 => Encoding.ASCII,
        65001 => Encoding.UTF8,
        _ => Encoding.GetEncoding(codePage)
    };
}
