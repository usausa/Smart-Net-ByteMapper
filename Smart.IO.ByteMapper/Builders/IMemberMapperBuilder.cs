namespace Smart.IO.ByteMapper.Builders
{
    using System.Reflection;

    using Smart.IO.ByteMapper.Mappers;

    public interface IMemberMapperBuilder
    {
        int Offset { get; set; }

        int CalcSize(PropertyInfo pi);

        IMapper CreateMapper(IBuilderContext context, PropertyInfo pi);
    }
}
