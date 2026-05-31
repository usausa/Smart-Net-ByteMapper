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
    // Precomputed max buffer sizes to avoid per-call virtual dispatch on encoding.
    private readonly int readCharCount;
    private readonly int writeCharCount;
    private readonly int writeByteCount;

    public int Size { get; }

    public DateTimeTextConverter(
        int length,
        string format,
        int codePage = CodePages.Ascii,
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
        readCharCount = encoding.GetMaxCharCount(Size);
        writeCharCount = Math.Max(Size, 64);
        writeByteCount = encoding.GetMaxByteCount(writeCharCount);
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

        Span<char> chars = stackalloc char[readCharCount];
        var charCount = encoding.GetChars(source[..size], chars);
        return ParseValue(chars[..charCount]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, T value)
    {
        Span<char> chars = stackalloc char[writeCharCount];
        if (!TryFormatValue(value, chars, out var charsWritten))
        {
            destination[..Size].Fill(filler);
            return;
        }
        Span<byte> encoded = stackalloc byte[writeByteCount];
        var count = encoding.GetBytes(chars[..charsWritten], encoded);
        var written = Math.Min(count, Size);
        encoded[..written].CopyTo(destination[..written]);
        if (written < Size)
        {
            destination[written..Size].Fill(filler);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ParseValue(ReadOnlySpan<char> str)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFormatValue(T value, Span<char> buffer, out int charsWritten)
    {
        if (typeof(T) == typeof(DateTime))
        {
            return Unsafe.BitCast<T, DateTime>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(DateTimeOffset))
        {
            return Unsafe.BitCast<T, DateTimeOffset>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(DateOnly))
        {
            return Unsafe.BitCast<T, DateOnly>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(TimeOnly))
        {
            return Unsafe.BitCast<T, TimeOnly>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    private static Encoding ResolveEncoding(int codePage) => codePage switch
    {
        20127 => Encoding.ASCII,
        65001 => Encoding.UTF8,
        _ => Encoding.GetEncoding(codePage)
    };
}
