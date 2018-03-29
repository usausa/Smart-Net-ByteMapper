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

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(bool))
            {
                var trueValue = TrueValue ?? context.GetParameter<byte>(Parameter.TrueValue);
                var falseValue = FalseValue ?? context.GetParameter<byte>(Parameter.FalseValue);
                return new BoolConverter(trueValue, falseValue);
            }

            throw new InvalidOperationException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
