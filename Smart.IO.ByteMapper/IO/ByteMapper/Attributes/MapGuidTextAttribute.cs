namespace Smart.IO.ByteMapper.Attributes;

using System.Text;

using Smart.IO.ByteMapper.Builders;

public sealed class MapGuidTextAttribute : AbstractMemberMapAttribute
{
    private readonly GuidTextConverterBuilder builder = new();

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

    public byte Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapGuidTextAttribute(int offset, int length, string format)
        : base(offset)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
        builder.Format = format;
    }

    public override IMapConverterBuilder GetConverterBuilder() => builder;
}
