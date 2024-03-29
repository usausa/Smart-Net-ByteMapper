namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapDateTimeAttribute : AbstractMemberMapAttribute
{
    private readonly DateTimeConverterBuilder builder = new();

    public byte Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapDateTimeAttribute(int offset, string format)
        : base(offset)
    {
        builder.Format = format;
    }

    public MapDateTimeAttribute(int offset, string format, DateTimeKind kind)
        : base(offset)
    {
        builder.Format = format;
        builder.Kind = kind;
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
