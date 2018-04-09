namespace Smart.IO.Mapper.Builders
{
    using System.Reflection;

    using Smart.IO.Mapper.Mappers;

    public interface IMemberMapperBuilder
    {
        int Offset { get; set; }

        int CalcSize(PropertyInfo pi);

        IMapper CreateMapper(IBuilderContext context, PropertyInfo pi);
    }
}
