namespace Smart.IO.Mapper.Builders
{
    using System.Reflection;

    using Smart.IO.Mapper.Mappers;

    public class MemberMapperBuilder<T> : IMemberMapperBuilder
        where T : IMapConverterBuilder
    {
        public int Offset { get; set; }

        public T ConverterBuilder { get; }

        public MemberMapperBuilder(T converterBuilder)
        {
            ConverterBuilder = converterBuilder;
        }

        public int CalcSize(IBuilderContext context, PropertyInfo pi)
        {
            return ConverterBuilder.CalcSize(context, pi.PropertyType);
        }

        public IMapper CreateMapper(IBuilderContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }
    }
}
