namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class DateTimeTextAttribute : AbstractPropertyAttribute
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public string Format { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public DateTimeTextAttribute(int offset)
            : base(offset)
        {
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeEncoding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
            var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider);

            if ((pi.PropertyType == typeof(DateTime)) || (pi.PropertyType == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(Length, encoding, filler, Format, style, provider, pi.PropertyType);
            }

            if ((pi.PropertyType == typeof(DateTimeOffset)) || (pi.PropertyType == typeof(DateTime?)))
            {
                return new DateTimeOffsetTextConverter(Length, encoding, filler, Format, style, provider, pi.PropertyType);
            }

            throw new InvalidOperationException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
