namespace Smart.IO.ByteMapper.Expressions;

using Smart.IO.ByteMapper.Builders;

public interface IMapGuidSyntax
{
    IMapGuidSyntax Endian(Endian value);
}

internal sealed class MapGuidExpression : IMemberMapExpression, IMapGuidSyntax
{
    private readonly GuidConverterBuilder builder = new();

    public IMapGuidSyntax Endian(Endian value)
    {
        builder.Endian = value;
        return this;
    }

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder() => builder;
}
