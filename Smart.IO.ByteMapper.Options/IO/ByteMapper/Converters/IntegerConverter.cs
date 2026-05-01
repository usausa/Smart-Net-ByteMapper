namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class Int32Converter : IMapConverter
{
    private readonly int length;

    private readonly Padding padding;

    private readonly bool zerofill;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int32Converter(
        int length,
        Padding padding,
        bool zerofill,
        byte filler,
        Type type)
    {
        this.length = length;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (NumberByteHelper.TryParseInt32(buffer, 0, length, filler, out var result))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
            NumberByteHelper.FormatInt32(buffer, 0, length, (int)value, padding, zerofill, filler);
        }
    }
}

internal sealed class Int64Converter : IMapConverter
{
    private readonly int length;

    private readonly Padding padding;

    private readonly bool zerofill;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int64Converter(
        int length,
        Padding padding,
        bool zerofill,
        byte filler,
        Type type)
    {
        this.length = length;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (NumberByteHelper.TryParseInt64(buffer, 0, length, filler, out var result))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
            NumberByteHelper.FormatInt64(buffer, 0, length, (long)value, padding, zerofill, filler);
        }
    }
}

internal sealed class Int16Converter : IMapConverter
{
    private readonly int length;

    private readonly Padding padding;

    private readonly bool zerofill;

    private readonly byte filler;

    private readonly Type convertEnumType;

    private readonly object defaultValue;

    public Int16Converter(
        int length,
        Padding padding,
        bool zerofill,
        byte filler,
        Type type)
    {
        this.length = length;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
        convertEnumType = EnumHelper.GetConvertEnumType(type);
        defaultValue = type.GetDefaultValue();
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (NumberByteHelper.TryParseInt16(buffer, 0, length, filler, out var result))
        {
            return convertEnumType is null ? result : Enum.ToObject(convertEnumType, result);
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
            NumberByteHelper.FormatInt16(buffer, 0, length, (short)value, padding, zerofill, filler);
        }
    }
}
