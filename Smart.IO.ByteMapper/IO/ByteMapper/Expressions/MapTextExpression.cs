namespace Smart.IO.ByteMapper.Expressions;

using System;
using System.Text;

using Smart.IO.ByteMapper.Builders;

public interface IMapTextSyntax
{
    IMapTextSyntax Encoding(Encoding value);

    IMapTextSyntax Trim(bool value);

    IMapTextSyntax Padding(Padding value);

    IMapTextSyntax Filler(byte value);
}

internal sealed class MapTextExpression : IMemberMapExpression, IMapTextSyntax
{
    private readonly TextConverterBuilder builder = new();

    public MapTextExpression(int length)
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

    public IMapTextSyntax Encoding(Encoding value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        builder.Encoding = value;
        return this;
    }

    public IMapTextSyntax Trim(bool value)
    {
        builder.Trim = value;
        return this;
    }

    public IMapTextSyntax Padding(Padding value)
    {
        builder.Padding = value;
        return this;
    }

    public IMapTextSyntax Filler(byte value)
    {
        builder.Filler = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
