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
            ((TimeSpan)value).TryFormat(chars, out var charWritten, format, null);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
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
            ((DateOnly)value).TryFormat(chars, out var charWritten, format, null);
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
            ((TimeOnly)value).TryFormat(chars, out var charWritten, format, null);
            var written = encoding.GetBytes(chars[..charWritten], destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}
