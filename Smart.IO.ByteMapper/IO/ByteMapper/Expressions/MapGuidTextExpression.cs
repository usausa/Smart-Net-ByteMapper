namespace Smart.IO.ByteMapper.Expressions;

using System.Text;

using Smart.IO.ByteMapper.Builders;

public interface IMapGuidTextSyntax
{
    IMapGuidTextSyntax Encoding(Encoding value);

    IMapGuidTextSyntax Filler(byte value);
}

internal sealed class MapGuidTextExpression : IMemberMapExpression, IMapGuidTextSyntax
{
    private readonly GuidTextConverterBuilder builder = new();

    public MapGuidTextExpression(int length, string format)
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

    public IMapGuidTextSyntax Encoding(Encoding value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        builder.Encoding = value;
        return this;
    }

    public IMapGuidTextSyntax Filler(byte value)
    {
        builder.Filler = value;
        return this;
    }

    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
