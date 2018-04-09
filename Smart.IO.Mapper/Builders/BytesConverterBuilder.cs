namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class BytesConverterBuilder : IMapConverterBuilder
    {
        public int Length { get; set; }

        public byte? Filler { get; set; }

        public int CalcSize(Type type)
        {
            return Length;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if (type == typeof(byte[]))
            {
                return new BytesConverter(
                    Length,
                    Filler ?? context.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
