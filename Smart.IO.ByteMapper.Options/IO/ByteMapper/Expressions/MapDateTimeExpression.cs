namespace Smart.IO.ByteMapper.Expressions;

using System;

using Smart.IO.ByteMapper.Builders;

public interface IMapDateTimeSyntax
{
    IMapDateTimeSyntax Filler(byte value);
}

internal sealed class MapDateTimeExpression : IMemberMapExpression, IMapDateTimeSyntax
{
    private readonly DateTimeConverterBuilder builder = new();

    public MapDateTimeExpression(string format)
    {
        if (String.IsNullOrEmpty(format))
        {
            throw new ArgumentException("Invalid format.", nameof(format));
        }

        builder.Format = format;
    }

    public MapDateTimeExpression(string format, DateTimeKind kind)
    {
        if (String.IsNullOrEmpty(format))
        {
            throw new ArgumentException("Invalid format.", nameof(format));
        }

        builder.Format = format;
        builder.Kind = kind;
    }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public IMapDateTimeSyntax Filler(byte value)
    {
        builder.Filler = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    public IMapConverterBuilder GetMapConverterBuilder() => builder;
}
