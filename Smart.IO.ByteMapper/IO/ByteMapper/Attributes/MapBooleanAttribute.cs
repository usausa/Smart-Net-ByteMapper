namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapBooleanAttribute : AbstractMemberMapAttribute
{
    private readonly BooleanConverterBuilder builder = new();

    public byte TrueValue
    {
        get => throw new NotSupportedException();
        set => builder.TrueValue = value;
    }

    public byte FalseValue
    {
        get => throw new NotSupportedException();
        set => builder.FalseValue = value;
    }

    public byte NullValue
    {
        get => throw new NotSupportedException();
        set => builder.NullValue = value;
    }

    public MapBooleanAttribute(int offset)
        : base(offset)
    {
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
