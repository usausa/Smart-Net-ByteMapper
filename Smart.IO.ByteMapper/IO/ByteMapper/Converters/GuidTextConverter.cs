namespace Smart.IO.ByteMapper.Converters;

using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

using Smart.IO.ByteMapper.Helpers;

internal sealed class GuidTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    public GuidTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        if (Guid.TryParseExact(chars[..charCount].Trim(), format, out var result))
        {
            return result;
        }

        return default(Guid);
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
            return;
        }

        var destination = buffer[..length];
        Span<char> chars = stackalloc char[length + 8];
        ((Guid)value).TryFormat(chars, out var charWritten, format);
        var written = encoding.GetBytes(chars[..charWritten], destination);
        if (written < length)
        {
            destination[written..].Fill(filler);
        }
    }
}

internal sealed class NullableGuidTextConverter : IMapConverter
{
    private readonly int length;

    private readonly string format;

    private readonly Encoding encoding;

    private readonly byte filler;

    public NullableGuidTextConverter(
        int length,
        string format,
        Encoding encoding,
        byte filler)
    {
        this.length = length;
        this.format = format;
        this.encoding = encoding;
        this.filler = filler;
    }

    [SkipLocalsInit]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        Span<char> chars = stackalloc char[length];
        var charCount = encoding.GetChars(buffer[..length], chars);
        var trimmed = chars[..charCount].Trim();
        if (trimmed.IsEmpty)
        {
            return null;
        }

        if (Guid.TryParseExact(trimmed, format, out var result))
        {
            return (Guid?)result;
        }

        return null;
    }

    [SkipLocalsInit]
    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
            return;
        }

        var destination = buffer[..length];
        Span<char> chars = stackalloc char[length + 8];
        ((Guid)(Guid?)value).TryFormat(chars, out var charWritten, format);
        var written = encoding.GetBytes(chars[..charWritten], destination);
        if (written < length)
        {
            destination[written..].Fill(filler);
        }
    }
}

internal sealed class GuidTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly char formatChar;

    private readonly int width;

    private readonly StandardFormat standardFormat;

    private readonly byte filler;

    public GuidTextFastConverter(int length, char formatChar, int width, byte filler)
    {
        this.length = length;
        this.formatChar = formatChar;
        this.width = width;
        standardFormat = new StandardFormat(formatChar);
        this.filler = filler;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return Utf8Parser.TryParse(buffer[..width], out Guid result, out _, formatChar) ? result : default;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Utf8Formatter.TryFormat((Guid)value, buffer[..width], out _, standardFormat);
            if (width < length)
            {
                buffer[width..length].Fill(filler);
            }
        }
    }
}

internal sealed class NullableGuidTextFastConverter : IMapConverter
{
    private readonly int length;

    private readonly char formatChar;

    private readonly int width;

    private readonly StandardFormat standardFormat;

    private readonly byte filler;

    public NullableGuidTextFastConverter(int length, char formatChar, int width, byte filler)
    {
        this.length = length;
        this.formatChar = formatChar;
        this.width = width;
        standardFormat = new StandardFormat(formatChar);
        this.filler = filler;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        return Utf8Parser.TryParse(buffer[..width], out Guid result, out _, formatChar) ? (Guid?)result : null;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            Utf8Formatter.TryFormat((Guid)(Guid?)value, buffer[..width], out _, standardFormat);
            if (width < length)
            {
                buffer[width..length].Fill(filler);
            }
        }
    }
}
