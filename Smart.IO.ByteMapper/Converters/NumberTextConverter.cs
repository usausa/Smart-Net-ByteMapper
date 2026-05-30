namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

public sealed class NumberTextConverter<T>
    where T : struct
{
    private readonly Encoding encoding;
    private readonly string? format;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;
    private readonly NumberStyles style;
    private readonly IFormatProvider provider;
    // Precomputed max buffer sizes to avoid per-call virtual dispatch on encoding.
    private readonly int readCharCount;
    private readonly int writeCharCount;
    private readonly int writeByteCount;

    public int Size { get; }

    public NumberTextConverter(
        int length,
        string? format = null,
        int codePage = 20127,
        bool trim = true,
        Padding padding = Padding.Left,
        byte filler = 0x20,
        NumberStyles style = NumberStyles.Integer,
        Culture culture = Culture.Invariant)
    {
        Size = length;
        this.format = format;
        encoding = ResolveEncoding(codePage);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        this.style = style;
        provider = culture == Culture.Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
        readCharCount = encoding.GetMaxCharCount(Size);
        writeCharCount = Math.Max(Size, 48);
        writeByteCount = encoding.GetMaxByteCount(writeCharCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(ReadOnlySpan<byte> source)
    {
        var start = 0;
        var size = Size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(source, ref start, ref size, padding, filler);
        }
        if (size == 0)
        {
            return default;
        }
        Span<char> chars = stackalloc char[readCharCount];
        var charCount = encoding.GetChars(source.Slice(start, size), chars);
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
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, Size, padding, filler);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ParseValue(ReadOnlySpan<char> str)
    {
        if (typeof(T) == typeof(int))
        {
            return Unsafe.BitCast<int, T>(Int32.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(long))
        {
            return Unsafe.BitCast<long, T>(Int64.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(short))
        {
            return Unsafe.BitCast<short, T>(Int16.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(float))
        {
            return Unsafe.BitCast<float, T>(Single.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(double))
        {
            return Unsafe.BitCast<double, T>(Double.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(decimal))
        {
            return Unsafe.BitCast<decimal, T>(Decimal.Parse(str, style, provider));
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFormatValue(T value, Span<char> buffer, out int charsWritten)
    {
        if (typeof(T) == typeof(int))
        {
            return Unsafe.BitCast<T, int>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(long))
        {
            return Unsafe.BitCast<T, long>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(short))
        {
            return Unsafe.BitCast<T, short>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(float))
        {
            return Unsafe.BitCast<T, float>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(double))
        {
            return Unsafe.BitCast<T, double>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(decimal))
        {
            return Unsafe.BitCast<T, decimal>(value).TryFormat(buffer, out charsWritten, format, provider);
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
