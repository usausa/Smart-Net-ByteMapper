namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class DateTimeConverterBuilder : AbstractMapConverterBuilder<DateTimeConverterBuilder>
{
    public string Format { get; set; }

    public DateTimeKind? Kind { get; set; }

    public byte? Filler { get; set; }

    static DateTimeConverterBuilder()
    {
        AddEntry(typeof(DateTime), static (b, _) => b.Format.Length, static (b, t, c) => b.CreateDateTimeConverter(t, c));
        AddEntry(typeof(DateTime?), static (b, _) => b.Format.Length, static (b, t, c) => b.CreateDateTimeConverter(t, c));
        AddEntry(typeof(DateTimeOffset), static (b, _) => b.Format.Length, static (b, t, c) => b.CreateDateTimeOffsetConverter(t, c));
        AddEntry(typeof(DateTimeOffset?), static (b, _) => b.Format.Length, static (b, t, c) => b.CreateDateTimeOffsetConverter(t, c));
    }

    private DateTimeConverter CreateDateTimeConverter(Type type, IBuilderContext context)
    {
        return new DateTimeConverter(
            Format,
            Kind ?? context.GetParameter<DateTimeKind>(Parameter.DateTimeKind),
            Filler ?? context.GetParameter<byte>(Parameter.Filler),
            type);
    }

    private DateTimeOffsetConverter CreateDateTimeOffsetConverter(Type type, IBuilderContext context)
    {
        return new DateTimeOffsetConverter(
            Format,
            Kind ?? context.GetParameter<DateTimeKind>(Parameter.DateTimeKind),
            Filler ?? context.GetParameter<byte>(Parameter.Filler),
            type);
    }
}
