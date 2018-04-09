namespace Smart.IO.Mapper.Builders
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public class DateTimeTextConverterBuilder : IMapConverterBuilder
    {
        public int Length { get; set; }

        public string Format { get; set; }

        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public int CalcSize(IBuilderContext context, Type type)
        {
            return Length;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Filler ?? context.GetParameter<byte>(Parameter.Filler),
                    Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider),
                    type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTimeOffset?)))
            {
                return new DateTimeOffsetTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Filler ?? context.GetParameter<byte>(Parameter.Filler),
                    Style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.DateTimeProvider),
                    type);
            }

            return null;
        }
    }
}
