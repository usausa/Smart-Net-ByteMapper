namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;
    using Smart.Reflection;

    public sealed class ArrayConverterBuilder : IMapConverterBuilder
    {
        public int Length { get; set; }

        public byte? Filler { get; set; }

        public IMapConverterBuilder ElementConverterBuilder { get; set; }

        public int CalcSize(Type type)
        {
            return Length * ElementConverterBuilder.CalcSize(type);
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            var delegateFactory = context.Components.Get<IDelegateFactory>();
            return new ArrayConverter(
                delegateFactory.CreateArrayAllocator(type),
                Length,
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                ElementConverterBuilder.CalcSize(type),
                ElementConverterBuilder.CreateConverter(context, type));
        }
    }
}
