namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
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

        public override int CalcSize(Type type)
        {
            return Length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.NumberEncoding);
            var trim = Trim ?? context.GetParameter<bool>(Parameter.Trim);
            var padding = Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding);
            var filler = Filler ?? context.GetParameter<byte>(Parameter.NumberFiller);
            var provider = Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider);

            if ((type == typeof(int)) || (type == typeof(int?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new IntTextConverter(Length, encoding, trim, padding, filler, style, provider, type);
            }

            if ((type == typeof(long)) || (type == typeof(long?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new LongTextConverter(Length, encoding, trim, padding, filler, style, provider, type);
            }

            if ((type == typeof(short)) || (type == typeof(short?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle);
                return new ShortTextConverter(Length, encoding, trim, padding, filler, style, provider, type);
            }

            if ((type == typeof(decimal)) || (type == typeof(decimal?)))
            {
                var style = Style ?? context.GetParameter<NumberStyles>(Parameter.DecimalStyle);
                return new DecimalTextConverter(Length, encoding, trim, padding, filler, style, provider, type);
            }

            return null;
        }
    }
}
