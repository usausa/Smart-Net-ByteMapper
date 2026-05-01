namespace Smart.IO.ByteMapper.Converters;

using System.Globalization;
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

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var value = encoding.GetString(buffer[..length]);
        if (DateTime.TryParseExact(value, format, provider, style, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            var destination = buffer[..length];
            var written = encoding.GetBytes(((DateTime)value).ToString(format, provider), destination);
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

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var value = encoding.GetString(buffer[..length]);
        if (DateTimeOffset.TryParseExact(value, format, provider, style, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..length], filler);
        }
        else
        {
            var destination = buffer[..length];
            var written = encoding.GetBytes(((DateTimeOffset)value).ToString(format, provider), destination);
            if (written < length)
            {
                destination[written..].Fill(filler);
            }
        }
    }
}
