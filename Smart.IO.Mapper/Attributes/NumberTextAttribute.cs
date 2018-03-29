namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class NumberTextAttribute : AbstractPropertyAttribute
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public NumberStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public NumberTextAttribute(int offset)
            : base(offset)
        {
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.NumberEncoding);
            var trim = Trim ?? context.GetParameter<bool>(Parameter.Trim);
            var padding = Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.NumberFiller);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider);

            if ((pi.PropertyType == typeof(int)) || (pi.PropertyType == typeof(int?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new IntTextConverter(Length, encoding, trim, padding, filler, style, provider, pi.PropertyType);
            }

            if ((pi.PropertyType == typeof(long)) || (pi.PropertyType == typeof(long?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new LongTextConverter(Length, encoding, trim, padding, filler, style, provider, pi.PropertyType);
            }

            if ((pi.PropertyType == typeof(short)) || (pi.PropertyType == typeof(short?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new ShortTextConverter(Length, encoding, trim, padding, filler, style, provider, pi.PropertyType);
            }

            if ((pi.PropertyType == typeof(decimal)) || (pi.PropertyType == typeof(decimal?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.DecimalStyle);
                return new DecimalTextConverter(Length, encoding, trim, padding, filler, style, provider, pi.PropertyType);
            }

            throw new InvalidOperationException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
