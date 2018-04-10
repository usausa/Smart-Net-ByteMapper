namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public sealed class BooleanConverterBuilder : IMapConverterBuilder
    {
        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public byte? NullValue { get; set; }

        public int CalcSize(Type type)
        {
            return 1;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if (type == typeof(bool))
            {
                return new BooleanConverter(
                    TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                    FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue));
            }

            if (type == typeof(bool?))
            {
                return new NullableBooleanConverter(
                    TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                    FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue),
                    NullValue ?? context.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
