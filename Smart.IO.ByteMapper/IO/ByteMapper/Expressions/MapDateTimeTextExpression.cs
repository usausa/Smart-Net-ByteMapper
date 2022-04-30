namespace Smart.IO.ByteMapper.Expressions;

using System.Globalization;
using System.Text;

using Smart.IO.ByteMapper.Builders;

public interface IMapDateTimeTextSyntax
{
    IMapDateTimeTextSyntax Encoding(Encoding value);

    IMapDateTimeTextSyntax Filler(byte value);

    IMapDateTimeTextSyntax Style(DateTimeStyles value);

    IMapDateTimeTextSyntax Provider(IFormatProvider value);
}

internal sealed class MapDateTimeTextExpression : IMemberMapExpression, IMapDateTimeTextSyntax
{
    private readonly DateTimeTextConverterBuilder builder = new();

    public MapDateTimeTextExpression(int length, string format)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
        builder.Format = format;
    }

    //--------------------------------------------------------------------------------
    // Syntax
    //--------------------------------------------------------------------------------

    public IMapDateTimeTextSyntax Encoding(Encoding value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        builder.Encoding = value;
        return this;
    }

    public IMapDateTimeTextSyntax Filler(byte value)
    {
        builder.Filler = value;
        return this;
    }

    public IMapDateTimeTextSyntax Style(DateTimeStyles value)
    {
        builder.Style = value;
        return this;
    }

    public IMapDateTimeTextSyntax Provider(IFormatProvider value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        builder.Provider = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
