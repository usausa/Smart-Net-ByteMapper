namespace Smart.IO.ByteMapper.Converters;

using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using Smart.IO.ByteMapper.Helpers;

internal static class NumberTextConverterHelper
{
    // Map a numeric format string (e.g. "D2", "N", null) to a Utf8Formatter StandardFormat.
    // Utf8Formatter supports: 'D' (decimal integer), 'G', 'N', 'X' (hex), 'E' (scientific).
    // For unsupported specifiers the default (no format) is used.
    internal static StandardFormat ParseStandardFormat(string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return default;
        }

        var symbol = char.ToUpperInvariant(format[0]);
        if (symbol is 'D' or 'G' or 'N' or 'X' or 'E')
        {
            if (format.Length == 1)
            {
                return new StandardFormat(symbol);
            }

            if (format.Length > 1 && byte.TryParse(format.AsSpan(1), out var precision))
            {
                return new StandardFormat(symbol, precision);
            }
        }

        return default;
    }
}

internal sealed class Int32TextConverter : IMapConverter
{
    private readonly int length;

    private readonly StandardFormat standardFormat;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int32TextConverter(
        int length,
        string format,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        standardFormat = NumberTextConverterHelper.ParseStandardFormat(format);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count > 0 && Utf8Parser.TryParse(buffer.Slice(start, count), out int result, out _))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<byte> tmp = stackalloc byte[32];
            Utf8Formatter.TryFormat((int)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}

internal sealed class Int64TextConverter : IMapConverter
{
    private readonly int length;

    private readonly StandardFormat standardFormat;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int64TextConverter(
        int length,
        string format,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        standardFormat = NumberTextConverterHelper.ParseStandardFormat(format);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count > 0 && Utf8Parser.TryParse(buffer.Slice(start, count), out long result, out _))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<byte> tmp = stackalloc byte[32];
            Utf8Formatter.TryFormat((long)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}

internal sealed class Int16TextConverter : IMapConverter
{
    private readonly int length;

    private readonly StandardFormat standardFormat;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int16TextConverter(
        int length,
        string format,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        standardFormat = NumberTextConverterHelper.ParseStandardFormat(format);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count > 0 && Utf8Parser.TryParse(buffer.Slice(start, count), out short result, out _))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<byte> tmp = stackalloc byte[16];
            Utf8Formatter.TryFormat((short)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}

internal sealed class DecimalTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly NumberStyles style;

    private readonly IFormatProvider provider;

    private readonly object defaultValue;

    public DecimalTextConverter(
        int length,
        string format,
        Encoding encoding,
        bool trim,
        Padding padding,
        byte filler,
        NumberStyles style,
        IFormatProvider provider,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        this.style = style;
        this.provider = provider;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count == 0)
        {
            return defaultValue;
        }

        Span<char> chars = stackalloc char[count];
        var charCount = encoding.GetChars(buffer.Slice(start, count), chars);
        if (charCount > 0 && Decimal.TryParse(chars[..charCount], style, provider, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<char> chars = stackalloc char[length + 32];
            ((decimal)value).TryFormat(chars, out var charWritten, format, provider);
            var maxBytes = encoding.GetMaxByteCount(charWritten);
            var tmp = maxBytes <= 256 ? stackalloc byte[maxBytes] : new byte[maxBytes];
            var byteWritten = encoding.GetBytes(chars[..charWritten], tmp);
            BytesHelper.CopyBytes(tmp[..byteWritten], buffer, length, padding, filler);
        }
    }
}

internal sealed class FloatTextConverter : IMapConverter
{
    private readonly int length;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly object defaultValue;

    public FloatTextConverter(
        int length,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count > 0 && Utf8Parser.TryParse(buffer.Slice(start, count), out float result, out _))
        {
            return result;
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<byte> tmp = stackalloc byte[32];
            Utf8Formatter.TryFormat((float)value, tmp, out var written);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}

internal sealed class DoubleTextConverter : IMapConverter
{
    private readonly int length;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly object defaultValue;

    public DoubleTextConverter(
        int length,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        var count = length;
        if (trim)
        {
            BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
        }

        if (count > 0 && Utf8Parser.TryParse(buffer.Slice(start, count), out double result, out _))
        {
            return result;
        }

        return defaultValue;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Span<byte> tmp = stackalloc byte[32];
            Utf8Formatter.TryFormat((double)value, tmp, out var written);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}
