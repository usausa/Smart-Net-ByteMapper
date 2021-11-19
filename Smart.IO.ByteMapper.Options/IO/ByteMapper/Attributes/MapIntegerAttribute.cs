namespace Smart.IO.ByteMapper.Attributes;

using System;

using Smart.IO.ByteMapper.Builders;

public sealed class MapIntegerAttribute : AbstractMemberMapAttribute
{
    private readonly IntegerConverterBuilder builder = new();

    public Padding Padding
    {
        get => throw new NotSupportedException();
        set => builder.Padding = value;
    }

    public bool ZeroFill
    {
        get => throw new NotSupportedException();
        set => builder.ZeroFill = value;
    }

    public byte Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapIntegerAttribute(int offset, int length)
        : base(offset)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
