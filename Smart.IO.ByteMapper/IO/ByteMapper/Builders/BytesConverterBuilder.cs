namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class BytesConverterBuilder : AbstractMapConverterBuilder<BytesConverterBuilder>
{
    public int Length { get; set; }

    public byte? Filler { get; set; }

    static BytesConverterBuilder()
    {
        AddEntry(typeof(byte[]), static (b, _) => b.Length, static (b, _, c) => b.CreateBytesConverter(c));
    }

    private BytesConverter CreateBytesConverter(IBuilderContext context)
    {
        return new BytesConverter(
            Length,
            Filler ?? context.GetParameter<byte>(Parameter.Filler));
    }
}
