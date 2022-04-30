namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public abstract class AbstractTypeMapAttribute : Attribute
{
    public abstract ITypeMapperBuilder GetTypeMapperBuilder();
}
