namespace Smart.IO.ByteMapper.Builders;

using System.Text;

using Smart.IO.ByteMapper.Converters;

public sealed class TextConverterBuilder : AbstractMapConverterBuilder<TextConverterBuilder>
{
    public int Length { get; set; }

    public Encoding Encoding { get; set; }

    public bool? Trim { get; set; }

    public Padding? Padding { get; set; }

    public byte? Filler { get; set; }

    static TextConverterBuilder()
    {
        AddEntry(typeof(string), static (b, _) => b.Length, static (b, _, c) => b.CreateTextConverter(c));
    }

    private TextConverter CreateTextConverter(IBuilderContext context)
    {
        return new TextConverter(
            Length,
            Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
            Trim ?? context.GetParameter<bool>(Parameter.Trim),
            Padding ?? context.GetParameter<Padding>(Parameter.TextPadding),
            Filler ?? context.GetParameter<byte>(Parameter.TextFiller));
    }
}
