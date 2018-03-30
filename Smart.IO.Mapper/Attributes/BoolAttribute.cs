namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class BoolAttribute : AbstractPropertyAttribute
    {
        private byte? trueValue;

        private byte? falseValue;

        private byte? nullValue;

        public byte TrueValue
        {
            get => throw new NotSupportedException();
            set => trueValue = value;
        }

        public byte FalseValue
        {
            get => throw new NotSupportedException();
            set => falseValue = value;
        }

        public byte NullValue
        {
            get => throw new NotSupportedException();
            set => nullValue = value;
        }

        public BoolAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            return 1;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            if (type == typeof(bool))
            {
                return new BoolConverter(
                    trueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                    falseValue ?? context.GetParameter<byte>(Parameter.FalseValue));
            }

            if (type == typeof(bool?))
            {
                return new NullableBoolConverter(
                    trueValue ?? context.GetParameter<byte>(Parameter.TrueValue),
                    falseValue ?? context.GetParameter<byte>(Parameter.FalseValue),
                    nullValue ?? context.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
