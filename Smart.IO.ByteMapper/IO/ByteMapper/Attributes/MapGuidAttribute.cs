namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapGuidAttribute : AbstractMemberMapAttribute
{
    private readonly GuidConverterBuilder builder = new();

    public Endian Endian
    {
        get => throw new NotSupportedException();
        set => builder.Endian = value;
    }

    public MapGuidAttribute(int offset)
        : base(offset)
    {
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
