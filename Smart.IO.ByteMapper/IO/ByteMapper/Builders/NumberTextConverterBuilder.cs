namespace Smart.IO.ByteMapper.Builders
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Converters;

    public sealed class NumberTextConverterBuilder : AbstractMapConverterBuilder<NumberTextConverterBuilder>
    {
        public int Length { get; set; }

        public string Format { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public NumberStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        static NumberTextConverterBuilder()
        {
            AddEntry(typeof(int), (b, _) => b.Length, (b, t, c) => b.CreateIntTextConverter(t, c));
            AddEntry(typeof(int?), (b, _) => b.Length, (b, t, c) => b.CreateIntTextConverter(t, c));
            AddEntry(typeof(long), (b, _) => b.Length, (b, t, c) => b.CreateLongTextConverter(t, c));
            AddEntry(typeof(long?), (b, _) => b.Length, (b, t, c) => b.CreateLongTextConverter(t, c));
            AddEntry(typeof(short), (b, _) => b.Length, (b, t, c) => b.CreateShortTextConverter(t, c));
            AddEntry(typeof(short?), (b, _) => b.Length, (b, t, c) => b.CreateShortTextConverter(t, c));
            AddEntry(typeof(decimal), (b, _) => b.Length, (b, t, c) => b.CreateDecimalTextConverter(t, c));
            AddEntry(typeof(decimal?), (b, _) => b.Length, (b, t, c) => b.CreateDecimalTextConverter(t, c));
        }

        private IMapConverter CreateIntTextConverter(Type type, IBuilderContext context)
        {
            return new Int32TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.NumberTextEncoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(Parameter.NumberTextPadding),
                Filler ?? context.GetParameter<byte>(Parameter.NumberTextFiller),
                Style ?? context.GetParameter<NumberStyles>(Parameter.NumberTextNumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberTextProvider),
                type);
        }

        private IMapConverter CreateLongTextConverter(Type type, IBuilderContext context)
        {
            return new Int64TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.NumberTextEncoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(Parameter.NumberTextPadding),
                Filler ?? context.GetParameter<byte>(Parameter.NumberTextFiller),
                Style ?? context.GetParameter<NumberStyles>(Parameter.NumberTextNumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberTextProvider),
                type);
        }

        private IMapConverter CreateShortTextConverter(Type type, IBuilderContext context)
        {
            return new Int16TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.NumberTextEncoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(Parameter.NumberTextPadding),
                Filler ?? context.GetParameter<byte>(Parameter.NumberTextFiller),
                Style ?? context.GetParameter<NumberStyles>(Parameter.NumberTextNumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberTextProvider),
                type);
        }

        private IMapConverter CreateDecimalTextConverter(Type type, IBuilderContext context)
        {
            return new DecimalTextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(Parameter.NumberTextEncoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(Parameter.NumberTextPadding),
                Filler ?? context.GetParameter<byte>(Parameter.NumberTextFiller),
                Style ?? context.GetParameter<NumberStyles>(Parameter.NumberTextDecimalStyle),
                Provider ?? context.GetParameter<IFormatProvider>(Parameter.NumberTextProvider),
                type);
        }
    }
}
