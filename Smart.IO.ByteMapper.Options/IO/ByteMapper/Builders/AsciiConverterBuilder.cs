namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class AsciiConverterBuilder : AbstractMapConverterBuilder<AsciiConverterBuilder>
{
    public int Length { get; set; }

    public bool? Trim { get; set; }

    public Padding? Padding { get; set; }

    public byte? Filler { get; set; }

    static AsciiConverterBuilder()
    {
        AddEntry(typeof(string), (b, _) => b.Length, (b, _, c) => b.CreateAsciiConverter(c));
    }

    private IMapConverter CreateAsciiConverter(IBuilderContext context)
    {
        return new AsciiConverter(
            Length,
            Trim ?? context.GetParameter<bool>(Parameter.Trim),
            Padding ?? context.GetParameter<Padding>(Parameter.TextPadding),
            Filler ?? context.GetParameter<byte>(Parameter.TextFiller));
    }
}
