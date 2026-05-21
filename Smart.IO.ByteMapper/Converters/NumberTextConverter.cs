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
        this.encoding = ResolveEncoding(codePage);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        this.style = style;
        this.provider = culture == Culture.Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
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
        var str = encoding.GetString(source.Slice(start, size));
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
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, Size, padding, filler);
    }

    private T ParseValue(string str)
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

    private string FormatValue(T value)
    {
        if (value is IFormattable f)
        {
            return f.ToString(format, provider);
        }

        return value.ToString() ?? string.Empty;
    }

    private static Encoding ResolveEncoding(int codePage) => codePage switch
    {
        20127 => Encoding.ASCII,
        65001 => Encoding.UTF8,
        _ => Encoding.GetEncoding(codePage)
    };
}
