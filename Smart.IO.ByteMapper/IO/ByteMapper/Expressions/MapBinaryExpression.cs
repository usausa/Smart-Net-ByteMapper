namespace Smart.IO.ByteMapper.Expressions;

using Smart.IO.ByteMapper.Builders;

public interface IMapBinarySyntax
{
    IMapBinarySyntax Endian(Endian value);
}

internal sealed class MapBinaryExpression : IMemberMapExpression, IMapBinarySyntax
{
    private readonly BinaryConverterBuilder builder = new();

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public IMapBinarySyntax Endian(Endian value)
    {
        builder.Endian = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
