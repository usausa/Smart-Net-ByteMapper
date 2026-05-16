namespace Smart.IO.ByteMapper.Builders;

using System.Text;

using Smart.IO.ByteMapper.Converters;

public sealed class GuidTextConverterBuilder : AbstractMapConverterBuilder<GuidTextConverterBuilder>
{
    public int Length { get; set; }

    public string Format { get; set; }

    public Encoding Encoding { get; set; }

    public byte? Filler { get; set; }

    static GuidTextConverterBuilder()
    {
        AddEntry(typeof(Guid), static (b, _) => b.Length, static (b, _, c) => b.CreateGuidTextConverter(c));
        AddEntry(typeof(Guid?), static (b, _) => b.Length, static (b, _, c) => b.CreateNullableGuidTextConverter(c));
    }

    private GuidTextConverter CreateGuidTextConverter(IBuilderContext context)
    {
        return new GuidTextConverter(
            Length,
            Format,
            Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
            Filler ?? context.GetParameter<byte>(Parameter.Filler));
    }

    private NullableGuidTextConverter CreateNullableGuidTextConverter(IBuilderContext context)
    {
        return new NullableGuidTextConverter(
            Length,
            Format,
            Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
            Filler ?? context.GetParameter<byte>(Parameter.Filler));
    }
}
