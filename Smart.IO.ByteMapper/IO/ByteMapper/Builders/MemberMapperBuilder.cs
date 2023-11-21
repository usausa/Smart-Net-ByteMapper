namespace Smart.IO.ByteMapper.Builders;

using System.Reflection;

using Smart.IO.ByteMapper.Mappers;
using Smart.Reflection;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
public sealed class MemberMapperBuilder : IMemberMapperBuilder
{
    private readonly IMapConverterBuilder converterBuilder;

    public int Offset { get; set; }

    public PropertyInfo Property { get; set; }

    public MemberMapperBuilder(IMapConverterBuilder converterBuilder)
    {
        this.converterBuilder = converterBuilder;
    }

    public int CalcSize()
    {
        return converterBuilder.CalcSize(Property.PropertyType);
    }

    public IMapper CreateMapper(IBuilderContext context)
    {
        var delegateFactory = context.Components.Get<IDelegateFactory>();
        return new MemberMapper(
            Offset,
            converterBuilder.CreateConverter(context, Property.PropertyType),
            delegateFactory.CreateGetter(Property),
            delegateFactory.CreateSetter(Property));
    }
}
