namespace Smart.IO.ByteMapper.Attributes;

using System;

using Smart.IO.ByteMapper.Builders;

public sealed class MapAsciiAttribute : AbstractMemberMapAttribute
{
    private readonly AsciiConverterBuilder builder = new();

    public bool Trim
    {
        get => throw new NotSupportedException();
        set => builder.Trim = value;
    }

    public Padding Padding
    {
        get => throw new NotSupportedException();
        set => builder.Padding = value;
    }

    public byte Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapAsciiAttribute(int offset, int length)
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
