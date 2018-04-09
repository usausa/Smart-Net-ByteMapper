namespace Smart.IO.Mapper.Builders
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public class NumberTextConverterBuilder : IMapConverterBuilder
    {
        public int Length { get; set; }

        public string Format { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public NumberStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public int CalcSize(Type type)
        {
            return Length;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if ((type == typeof(int)) || (type == typeof(int?)))
            {
                return new IntTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Trim ?? context.GetParameter<bool>(Parameter.Trim),
                    Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider),
                    type);
            }

            if ((type == typeof(long)) || (type == typeof(long?)))
            {
                return new LongTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Trim ?? context.GetParameter<bool>(Parameter.Trim),
                    Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider),
                    type);
            }

            if ((type == typeof(short)) || (type == typeof(short?)))
            {
                return new ShortTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Trim ?? context.GetParameter<bool>(Parameter.Trim),
                    Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    Style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider),
                    type);
            }

            if ((type == typeof(decimal)) || (type == typeof(decimal?)))
            {
                return new DecimalTextConverter(
                    Length,
                    Format,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Trim ?? context.GetParameter<bool>(Parameter.Trim),
                    Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    Style ?? context.GetParameter<NumberStyles>(Parameter.DecimalStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberProvider),
                    type);
            }

            return null;
        }
    }
}
