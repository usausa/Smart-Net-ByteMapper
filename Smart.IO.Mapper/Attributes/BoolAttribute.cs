namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class BoolBinaryAttribute : AbstractPropertyAttribute
    {
        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public BoolBinaryAttribute(int offset)
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
                var trueValue = TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue);
                var falseValue = FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue);
                return new BoolConverter(trueValue, falseValue);
            }

            return null;
        }
    }
}
