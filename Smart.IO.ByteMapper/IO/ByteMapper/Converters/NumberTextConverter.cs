namespace Smart.IO.ByteMapper.Converters;

using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

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
        if (symbol is 'D' or 'F' or 'G' or 'N' or 'X' or 'E')
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

    private readonly StandardFormat standardFormat;

    private readonly bool useUtf8Formatter;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly object defaultValue;

    public DecimalTextConverter(
        int length,
        string format,
        bool trim,
        Padding padding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.format = format;
        standardFormat = NumberTextConverterHelper.ParseStandardFormat(format);
        // Utf8Formatter supports standard single-char specifiers (D, F, G, N, E, X).
        // Custom format strings (e.g. "0.00") must fall back to decimal.TryFormat + ASCII encoding.
        useUtf8Formatter = string.IsNullOrEmpty(format) || standardFormat != default;
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

        if (count == 0)
        {
            return defaultValue;
        }

        var slice = buffer.Slice(start, count);
        if (Utf8Parser.TryParse(slice, out decimal result, out _))
        {
            return result;
        }

        // Fallback for values that Utf8Parser cannot handle (e.g. with custom format output)
        Span<char> chars = stackalloc char[count];
        var charCount = System.Text.Encoding.ASCII.GetChars(slice, chars);
        if (charCount > 0 && decimal.TryParse(chars[..charCount], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
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
        else if (useUtf8Formatter)
        {
            Span<byte> tmp = stackalloc byte[32];
            Utf8Formatter.TryFormat((decimal)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
        else
        {
            Span<char> chars = stackalloc char[length + 32];
            ((decimal)value).TryFormat(chars, out var charWritten, format, System.Globalization.CultureInfo.InvariantCulture);
            Span<byte> tmp = stackalloc byte[charWritten];
            System.Text.Encoding.ASCII.GetBytes(chars[..charWritten], tmp);
            BytesHelper.CopyBytes(tmp, buffer, length, padding, filler);
        }
    }
}

internal sealed class FloatTextConverter : IMapConverter
{
    private readonly int length;

    private readonly StandardFormat standardFormat;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly object defaultValue;

    public FloatTextConverter(
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
            Utf8Formatter.TryFormat((float)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}

internal sealed class DoubleTextConverter : IMapConverter
{
    private readonly int length;

    private readonly StandardFormat standardFormat;

    private readonly bool trim;

    private readonly Padding padding;

    private readonly byte filler;

    private readonly object defaultValue;

    public DoubleTextConverter(
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
            Utf8Formatter.TryFormat((double)value, tmp, out var written, standardFormat);
            BytesHelper.CopyBytes(tmp[..written], buffer, length, padding, filler);
        }
    }
}
