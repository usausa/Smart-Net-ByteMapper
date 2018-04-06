namespace Smart.IO.Mapper.Builders
{
    using System.Reflection;

    using Smart.IO.Mapper.Mappers;

    public class ArrayMemberMapperBuilder : IMemberMapperBuilder
    {
        public int Offset { get; set; }

        public IMapConverterBuilder ConverterBuilder { get; set; }

        public int CalcSize(IBuilderContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }

        public IMapper CreateMapper(IBuilderContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }
    }
}
