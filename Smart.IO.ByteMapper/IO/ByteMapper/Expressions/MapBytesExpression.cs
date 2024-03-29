namespace Smart.IO.ByteMapper.Expressions;

using Smart.IO.ByteMapper.Builders;

public interface IMapBytesSyntax
{
    IMapBytesSyntax Filler(byte value);
}

internal sealed class MapBytesExpression : IMemberMapExpression, IMapBytesSyntax
{
    private readonly BytesConverterBuilder builder = new();

    public MapBytesExpression(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
    }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public IMapBytesSyntax Filler(byte value)
    {
        builder.Filler = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
