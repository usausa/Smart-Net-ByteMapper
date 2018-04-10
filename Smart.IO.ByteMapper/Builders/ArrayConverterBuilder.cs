namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;
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
            var elementType = type.GetElementType();
            return new ArrayConverter(
                delegateFactory.CreateArrayAllocator(elementType),
                Length,
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                ElementConverterBuilder.CalcSize(elementType),
                ElementConverterBuilder.CreateConverter(context, elementType));
        }
    }
}
