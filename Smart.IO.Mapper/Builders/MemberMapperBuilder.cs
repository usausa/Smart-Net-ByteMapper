namespace Smart.IO.Mapper.Builders
{
    using System.Reflection;

    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    public sealed class MemberMapperBuilder<T> : IMemberMapperBuilder
        where T : IMapConverterBuilder
    {
        public int Offset { get; set; }

        public T ConverterBuilder { get; }

        public MemberMapperBuilder(T converterBuilder)
        {
            ConverterBuilder = converterBuilder;
        }

        public int CalcSize(PropertyInfo pi)
        {
            return ConverterBuilder.CalcSize(pi.PropertyType);
        }

        public IMapper CreateMapper(IBuilderContext context, PropertyInfo pi)
        {
            var delegateFactory = context.Components.Get<IDelegateFactory>();
            return new MemberMapper(
                Offset,
                ConverterBuilder.CreateConverter(context, pi.PropertyType),
                delegateFactory.CreateGetter(pi),
                delegateFactory.CreateSetter(pi));
        }
    }
}
