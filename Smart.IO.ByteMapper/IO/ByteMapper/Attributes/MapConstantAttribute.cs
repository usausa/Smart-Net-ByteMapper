namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

public sealed class MapConstantAttribute : AbstractTypeMapAttribute
{
    private readonly ConstantTypeMapperBuilder builder = new();

    public MapConstantAttribute(int offset, byte[] content)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        builder.Offset = offset;
        builder.Content = content;
    }

    public override ITypeMapperBuilder GetTypeMapperBuilder() => builder;
}
