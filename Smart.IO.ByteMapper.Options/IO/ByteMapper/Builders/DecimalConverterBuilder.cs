namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class DecimalConverterBuilder : AbstractMapConverterBuilder<DecimalConverterBuilder>
{
    public int Length { get; set; }

    public byte Scale { get; set; }

    public bool? UseGrouping { get; set; }

    public int GroupingSize { get; set; } = 3;

    public Padding? Padding { get; set; }

    public bool? ZeroFill { get; set; }

    public byte? Filler { get; set; }

    static DecimalConverterBuilder()
    {
        AddEntry(typeof(decimal), static (b, _) => b.Length, static (b, t, c) => b.CreateDecimalConverter(t, c));
        AddEntry(typeof(decimal?), static (b, _) => b.Length, static (b, t, c) => b.CreateDecimalConverter(t, c));
    }

    private DecimalConverter CreateDecimalConverter(Type type, IBuilderContext context)
    {
        return new DecimalConverter(
            Length,
            Scale,
            UseGrouping ?? context.GetParameter<bool>(OptionsParameter.UseGrouping) ? GroupingSize : 0,
            Padding ?? context.GetParameter<Padding>(OptionsParameter.NumberPadding),
            ZeroFill ?? context.GetParameter<bool>(OptionsParameter.ZeroFill),
            Filler ?? context.GetParameter<byte>(OptionsParameter.NumberFiller),
            type);
    }
}
