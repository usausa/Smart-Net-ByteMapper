namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Converters;

    public sealed class BooleanConverterBuilder : AbstractMapConverterBuilder<BooleanConverterBuilder>
    {
        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public byte? NullValue { get; set; }

        static BooleanConverterBuilder()
        {
            AddEntry(typeof(bool), 1, (b, t, c) => b.CreateBooleanConverter(c));
            AddEntry(typeof(bool?), 1, (b, t, c) => b.CreateNullableBooleanConverter(c));
        }

        private IMapConverter CreateBooleanConverter(IBuilderContext context)
        {
            return new BooleanConverter(
                TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue));
        }

        private IMapConverter CreateNullableBooleanConverter(IBuilderContext context)
        {
            return new NullableBooleanConverter(
                TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue),
                NullValue ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
