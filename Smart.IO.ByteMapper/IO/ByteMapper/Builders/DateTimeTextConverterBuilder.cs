namespace Smart.IO.ByteMapper.Builders;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Converters;

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
    }

    private DateTimeTextConverter CreateDateTimeTextConverter(Type type, IBuilderContext context)
    {
        return new DateTimeTextConverter(
            Length,
            Format,
            Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding),
            Filler ?? context.GetParameter<byte>(Parameter.Filler),
            Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeTextStyle),
            Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeTextProvider),
            type);
    }

    private DateTimeOffsetTextConverter CreateDateTimeOffsetTextConverter(Type type, IBuilderContext context)
    {
        return new DateTimeOffsetTextConverter(
            Length,
            Format,
            Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeTextEncoding),
            Filler ?? context.GetParameter<byte>(Parameter.Filler),
            Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeTextStyle),
            Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeTextProvider),
            type);
    }
}
