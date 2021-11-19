namespace Smart.IO.ByteMapper.Expressions;

using System;

using Smart.IO.ByteMapper.Builders;

public interface IMapArraySyntax
{
    void Filler(byte value);
}

internal sealed class MapArrayExpression : IMemberMapExpression, IMapArraySyntax
{
    private readonly ArrayConverterBuilder builder = new();

    public MapArrayExpression(int length, IMapConverterBuilder elementConverterBuilder)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
        builder.ElementConverterBuilder = elementConverterBuilder;
    }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public void Filler(byte value)
    {
        builder.Filler = value;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    public IMapConverterBuilder GetMapConverterBuilder() => builder;
}
