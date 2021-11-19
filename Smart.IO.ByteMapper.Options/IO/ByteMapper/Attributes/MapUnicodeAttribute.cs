namespace Smart.IO.ByteMapper.Attributes;

using System;

using Smart.IO.ByteMapper.Builders;

public sealed class MapUnicodeAttribute : AbstractMemberMapAttribute
{
    private readonly UnicodeConverterBuilder builder = new();

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

    public char Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapUnicodeAttribute(int offset, int length)
        : base(offset)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length % 2 != 0)
        {
            throw new ArgumentException("Invalid length.", nameof(length));
        }

        builder.Length = length;
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
