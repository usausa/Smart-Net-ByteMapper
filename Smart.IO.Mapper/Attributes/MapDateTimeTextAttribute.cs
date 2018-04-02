namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class MapDateTimeTextAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private readonly string format;

        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public MapDateTimeTextAttribute(int offset, int length, string format)
            : base(offset)
        {
            this.length = length;
            this.format = format;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeEncoding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
            var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider);

            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(length, encoding, filler, format, style, provider, type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTime?)))
            {
                return new DateTimeOffsetTextConverter(length, encoding, filler, format, style, provider, type);
            }

            return null;
        }
    }
}
