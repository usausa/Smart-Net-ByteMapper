namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using Smart.IO.ByteMapper.Helpers;

internal sealed class DateTimeTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    private readonly DateTimeStyles style;

    private readonly IFormatProvider provider;

    private readonly object defaultValue;

    public DateTimeTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler,
        DateTimeStyles style,
        IFormatProvider provider,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
        this.style = style;
        this.provider = provider;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (DateTime.TryParseExact(chars[..charCount], format, provider, style, out var result))
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
            var destination = buffer[..length];
            Span<char> chars = stackalloc char[length + 16];
            ((DateTime)value).TryFormat(chars, out var charWritten, format, provider);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}

internal sealed class DateTimeTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly DateTimeFormatFast formatter;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateTimeTextFastConverter(int length, DateTimeFormatFast formatter, byte filler, Type type)
    {
        this.length = length;
        this.formatter = formatter;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return formatter.TryParse(buffer[..length], out DateTime result) ? result : defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            formatter.TryFormat(buffer[..length], (DateTime)value);
            if (formatter.Width < length)
            {
                buffer[formatter.Width..length].Fill(filler);
            }
        }
    }
}

internal sealed class DateOnlyTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly DateTimeFormatFast formatter;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateOnlyTextFastConverter(int length, DateTimeFormatFast formatter, byte filler, Type type)
    {
        this.length = length;
        this.formatter = formatter;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return formatter.TryParse(buffer[..length], out DateOnly result) ? result : defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            formatter.TryFormat(buffer[..length], (DateOnly)value);
            if (formatter.Width < length)
            {
                buffer[formatter.Width..length].Fill(filler);
            }
        }
    }
}

internal sealed class TimeOnlyTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly DateTimeFormatFast formatter;

    private readonly byte filler;

    private readonly object defaultValue;

    public TimeOnlyTextFastConverter(int length, DateTimeFormatFast formatter, byte filler, Type type)
    {
        this.length = length;
        this.formatter = formatter;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return formatter.TryParse(buffer[..length], out TimeOnly result) ? result : defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            formatter.TryFormat(buffer[..length], (TimeOnly)value);
            if (formatter.Width < length)
            {
                buffer[formatter.Width..length].Fill(filler);
            }
        }
    }
}

internal sealed class DateTimeOffsetTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly DateTimeFormatFast formatter;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateTimeOffsetTextFastConverter(int length, DateTimeFormatFast formatter, byte filler, Type type)
    {
        this.length = length;
        this.formatter = formatter;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return formatter.TryParse(buffer[..length], out DateTimeOffset result) ? result : defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            formatter.TryFormat(buffer[..length], (DateTimeOffset)value);
            if (formatter.Width < length)
            {
                buffer[formatter.Width..length].Fill(filler);
            }
        }
    }
}

internal sealed class DateTimeOffsetTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    private readonly DateTimeStyles style;

    private readonly IFormatProvider provider;

    private readonly object defaultValue;

    public DateTimeOffsetTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler,
        DateTimeStyles style,
        IFormatProvider provider,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
        this.style = style;
        this.provider = provider;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (DateTimeOffset.TryParseExact(chars[..charCount], format, provider, style, out var result))
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
            var destination = buffer[..length];
            Span<char> chars = stackalloc char[length + 16];
            ((DateTimeOffset)value).TryFormat(chars, out var charWritten, format, provider);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}

internal sealed class TimeSpanTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    private readonly object defaultValue;

    public TimeSpanTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (TimeSpan.TryParseExact(chars[..charCount], format, null, out var result))
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
            var destination = buffer[..length];
            Span<char> chars = stackalloc char[length + 16];
            ((TimeSpan)value).TryFormat(chars, out var charWritten, format, CultureInfo.InvariantCulture);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}

internal sealed class TimeSpanTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly TimeSpanFormatFast formatter;

    private readonly byte filler;

    private readonly object defaultValue;

    public TimeSpanTextFastConverter(int length, TimeSpanFormatFast formatter, byte filler, Type type)
    {
        this.length = length;
        this.formatter = formatter;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
        => formatter.TryParse(buffer[..length], out var result) ? result : defaultValue;

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
            return;
        }

        if (!formatter.TryFormat(buffer[..formatter.Width], (TimeSpan)value))
        {
            BytesHelper.Fill(buffer[..length], filler);
            return;
        }

        if (formatter.Width < length)
        {
            buffer[formatter.Width..length].Fill(filler);
        }
    }
}

internal sealed class DateOnlyTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    private readonly object defaultValue;

    public DateOnlyTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (DateOnly.TryParseExact(chars[..charCount], format, null, DateTimeStyles.None, out var result))
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
            var destination = buffer[..length];
            Span<char> chars = stackalloc char[length + 16];
            ((DateOnly)value).TryFormat(chars, out var charWritten, format, CultureInfo.InvariantCulture);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}

internal sealed class TimeOnlyTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    private readonly object defaultValue;

    public TimeOnlyTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler,
        Type type)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
        defaultValue = type.GetDefaultValue();
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (TimeOnly.TryParseExact(chars[..charCount], format, null, DateTimeStyles.None, out var result))
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
            var destination = buffer[..length];
            Span<char> chars = stackalloc char[length + 16];
            ((TimeOnly)value).TryFormat(chars, out var charWritten, format, CultureInfo.InvariantCulture);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}
