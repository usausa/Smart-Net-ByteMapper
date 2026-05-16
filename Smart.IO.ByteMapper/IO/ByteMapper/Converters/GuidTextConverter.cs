namespace Smart.IO.ByteMapper.Converters;

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
