namespace Smart.IO.ByteMapper.Builders
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Converters;

    public sealed class DateTimeTextConverterBuilder : AbstractMapConverterBuilder<DateTimeTextConverterBuilder>
    {
        public int Length { get; set; }

        public string Format { get; set; }

        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        static DateTimeTextConverterBuilder()
        {
            AddEntry(typeof(DateTime), (b, t) => b.Length, (b, t, c) => b.CreateDateTimeTextConverter(t, c));
            AddEntry(typeof(DateTime?), (b, t) => b.Length, (b, t, c) => b.CreateDateTimeTextConverter(t, c));
            AddEntry(typeof(DateTimeOffset), (b, t) => b.Length, (b, t, c) => b.CreateDateTimeOffsetTextConverter(t, c));
            AddEntry(typeof(DateTimeOffset?), (b, t) => b.Length, (b, t, c) => b.CreateDateTimeOffsetTextConverter(t, c));
        }

        private IMapConverter CreateDateTimeTextConverter(Type type, IBuilderContext context)
        {
            return new DateTimeTextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeEncoding),
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider),
                type);
        }

        private IMapConverter CreateDateTimeOffsetTextConverter(Type type, IBuilderContext context)
        {
            return new DateTimeOffsetTextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.DateTimeEncoding),
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider),
                type);
        }
    }
}
