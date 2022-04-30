namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapDecimalAttribute : AbstractMemberMapAttribute
{
    private readonly DecimalConverterBuilder builder = new();

    public bool UseGrouping
    {
        get => throw new NotSupportedException();
        set => builder.UseGrouping = value;
    }

    public int GroupingSize
    {
        get => throw new NotSupportedException();
        set
        {
            builder.GroupingSize = value;
            builder.UseGrouping = value > 0;
        }
    }

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

    public MapDecimalAttribute(int offset, int length)
        : this(offset, length, 0)
    {
    }

    public MapDecimalAttribute(int offset, int length, byte scale)
        : base(offset)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if ((scale != 0) && (scale + 1 >= length))
        {
            throw new ArgumentException($"Invalid scale. length=[{length}], scale=[{scale}]");
        }

        builder.Length = length;
        builder.Scale = scale;
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
