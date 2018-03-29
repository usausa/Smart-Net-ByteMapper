namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
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

        public override int CalcSize(Type type)
        {
            return Length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeEncoding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
            var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider);

            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(Length, encoding, filler, Format, style, provider, type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTime?)))
            {
                return new DateTimeOffsetTextConverter(Length, encoding, filler, Format, style, provider, type);
            }

            return null;
        }
    }
}
