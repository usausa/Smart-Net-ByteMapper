namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class UnicodeConverterBuilder : AbstractMapConverterBuilder<UnicodeConverterBuilder>
{
    public int Length { get; set; }

    public bool? Trim { get; set; }

    public Padding? Padding { get; set; }

    public char? Filler { get; set; }

    static UnicodeConverterBuilder()
    {
        AddEntry(typeof(string), (b, _) => b.Length, (b, _, c) => b.CreateAsciiConverter(c));
    }

    private IMapConverter CreateAsciiConverter(IBuilderContext context)
    {
        return new UnicodeConverter(
            Length,
            Trim ?? context.GetParameter<bool>(Parameter.Trim),
            Padding ?? context.GetParameter<Padding>(Parameter.TextPadding),
            Filler ?? (context.TryGetParameter<char>(OptionsParameter.UnicodeFiller, out var filler)
                ? filler
                : (char)context.GetParameter<byte>(Parameter.TextFiller)));
    }
}
