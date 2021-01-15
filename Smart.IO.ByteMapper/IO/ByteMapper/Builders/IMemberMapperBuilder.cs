namespace Smart.IO.ByteMapper.Builders
{
    using System.Reflection;

    using Smart.IO.ByteMapper.Mappers;

    public interface IMemberMapperBuilder
    {
        int Offset { get; set; }

        PropertyInfo Property { get; set; }

        int CalcSize();

        IMapper CreateMapper(IBuilderContext context);
    }
}
