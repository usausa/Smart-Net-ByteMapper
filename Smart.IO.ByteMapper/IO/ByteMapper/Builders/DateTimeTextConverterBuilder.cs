namespace Smart.IO.ByteMapper.Builders;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Converters;
using Smart.IO.ByteMapper.Helpers;

public sealed class DateTimeTextConverterBuilder : AbstractMapConverterBuilder<DateTimeTextConverterBuilder>
{
    public int Length { get; set; }

    public string Format { get; set; }

    public Encoding Encoding { get; set; }

    public byte? Filler { get; set; }

    public DateTimeStyles? Style { get; set; }

    public IFormatProvider Provider { get; set; }

    static DateTimeTextConverterBuilder()
    {
        AddEntry(typeof(DateTime), static (b, _) => b.Length, static (b, t, c) => b.CreateDateTimeTextConverter(t, c));
        AddEntry(typeof(DateTime?), static (b, _) => b.Length, static (b, t, c) => b.CreateDateTimeTextConverter(t, c));
        AddEntry(typeof(DateTimeOffset), static (b, _) => b.Length, static (b, t, c) => b.CreateDateTimeOffsetTextConverter(t, c));
        AddEntry(typeof(DateTimeOffset?), static (b, _) => b.Length, static (b, t, c) => b.CreateDateTimeOffsetTextConverter(t, c));
        AddEntry(typeof(TimeSpan), static (b, _) => b.Length, static (b, t, c) => b.CreateTimeSpanTextConverter(t, c));
        AddEntry(typeof(TimeSpan?), static (b, _) => b.Length, static (b, t, c) => b.CreateTimeSpanTextConverter(t, c));
        AddEntry(typeof(DateOnly), static (b, _) => b.Length, static (b, t, c) => b.CreateDateOnlyTextConverter(t, c));
        AddEntry(typeof(DateOnly?), static (b, _) => b.Length, static (b, t, c) => b.CreateDateOnlyTextConverter(t, c));
        AddEntry(typeof(TimeOnly), static (b, _) => b.Length, static (b, t, c) => b.CreateTimeOnlyTextConverter(t, c));
        AddEntry(typeof(TimeOnly?), static (b, _) => b.Length, static (b, t, c) => b.CreateTimeOnlyTextConverter(t, c));
    }

    private IMapConverter CreateDateTimeTextConverter(Type type, IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
        var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeTextStyle);
        var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeTextProvider);

        if (style == DateTimeStyles.None)
        {
            var fast = DateTimeFormatFast.TryCreate(format, DateTimeFormatBlocks.Date | DateTimeFormatBlocks.Time);
            if (fast is not null && (fast.Width <= length) && (encoding.GetByteCount(format) == format.Length))
            {
                return new DateTimeTextFastConverter(length, fast, filler, type);
            }
        }

        return new DateTimeTextConverter(length, format, encoding, filler, style, provider, type);
    }

    private IMapConverter CreateDateTimeOffsetTextConverter(Type type, IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
        var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeTextStyle);
        var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeTextProvider);

        if (style == DateTimeStyles.None)
        {
            var fast = DateTimeFormatFast.TryCreate(format, DateTimeFormatBlocks.Date | DateTimeFormatBlocks.Time | DateTimeFormatBlocks.Offset);
            if (fast is not null && (fast.Width <= length) && (encoding.GetByteCount(format) == format.Length))
            {
                return new DateTimeOffsetTextFastConverter(length, fast, filler, type);
            }
        }

        return new DateTimeOffsetTextConverter(length, format, encoding, filler, style, provider, type);
    }

    private IMapConverter CreateTimeSpanTextConverter(Type type, IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);

        var fast = TimeSpanFormatFast.TryCreate(format);
        if (fast is not null && (fast.Width <= length) && (encoding.GetByteCount(format) == format.Length))
        {
            return new TimeSpanTextFastConverter(length, fast, filler, type);
        }

        return new TimeSpanTextConverter(length, format, encoding, filler, type);
    }

    private IMapConverter CreateDateOnlyTextConverter(Type type, IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);

        var fast = DateTimeFormatFast.TryCreate(format, DateTimeFormatBlocks.Date);
        if (fast is not null && (fast.Width <= length) && (encoding.GetByteCount(format) == format.Length))
        {
            return new DateOnlyTextFastConverter(length, fast, filler, type);
        }

        return new DateOnlyTextConverter(length, format, encoding, filler, type);
    }

    private IMapConverter CreateTimeOnlyTextConverter(Type type, IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);

        var fast = DateTimeFormatFast.TryCreate(format, DateTimeFormatBlocks.Time);
        if (fast is not null && (fast.Width <= length) && (encoding.GetByteCount(format) == format.Length))
        {
            return new TimeOnlyTextFastConverter(length, fast, filler, type);
        }

        return new TimeOnlyTextConverter(length, format, encoding, filler, type);
    }
}
