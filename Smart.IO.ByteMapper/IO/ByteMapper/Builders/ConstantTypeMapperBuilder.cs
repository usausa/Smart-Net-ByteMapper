namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Mappers;

#pragma warning disable CA1819
public sealed class ConstantTypeMapperBuilder : ITypeMapperBuilder
{
    public int Offset { get; set; }

    public byte[] Content { get; set; }

    public int CalcSize()
    {
        return Content.Length;
    }

    public IMapper CreateMapper(IBuilderContext context) => new ConstantMapper(Offset, Content);
}
#pragma warning restore CA1819
