namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapBinaryAttribute : AbstractMemberMapAttribute
{
    private readonly BinaryConverterBuilder builder = new();

    public Endian Endian
    {
        get => throw new NotSupportedException();
        set => builder.Endian = value;
    }

    public MapBinaryAttribute(int offset)
        : base(offset)
    {
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
