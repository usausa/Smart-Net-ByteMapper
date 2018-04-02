namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class MapDateTimeTextAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public string Format { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public MapDateTimeTextAttribute(int offset, int length)
            : base(offset)
        {
            this.length = length;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);
            var style = Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.Culture);

            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(length, encoding, filler, Format, style, provider, type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTime?)))
            {
                return new DateTimeOffsetTextConverter(length, encoding, filler, Format, style, provider, type);
            }

            return null;
        }
    }
}
