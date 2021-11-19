namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class BytesConverterBuilder : AbstractMapConverterBuilder<BytesConverterBuilder>
{
    public int Length { get; set; }

    public byte? Filler { get; set; }

    static BytesConverterBuilder()
    {
        AddEntry(typeof(byte[]), (b, _) => b.Length, (b, _, c) => b.CreateBytesConverter(c));
    }

    private IMapConverter CreateBytesConverter(IBuilderContext context)
    {
        return new BytesConverter(
            Length,
            Filler ?? context.GetParameter<byte>(Parameter.Filler));
    }
}
