namespace Smart.IO.ByteMapper.Attributes;

using System.Text;

using Smart.IO.ByteMapper.Builders;

public sealed class MapTextAttribute : AbstractMemberMapAttribute
{
    private readonly TextConverterBuilder builder = new();

    public int CodePage
    {
        get => throw new NotSupportedException();
        set => builder.Encoding = Encoding.GetEncoding(value);
    }

    public string EncodingName
    {
        get => throw new NotSupportedException();
        set => builder.Encoding = Encoding.GetEncoding(value);
    }

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

    public MapTextAttribute(int offset, int length)
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
