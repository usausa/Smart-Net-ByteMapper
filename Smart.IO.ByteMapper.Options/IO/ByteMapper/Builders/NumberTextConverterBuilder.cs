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
            AddEntry(typeof(int), (b, t) => b.Length, (b, t, c) => b.CreateIntTextConverter(t, c));
            AddEntry(typeof(int?), (b, t) => b.Length, (b, t, c) => b.CreateIntTextConverter(t, c));
            AddEntry(typeof(long), (b, t) => b.Length, (b, t, c) => b.CreateLongTextConverter(t, c));
            AddEntry(typeof(long?), (b, t) => b.Length, (b, t, c) => b.CreateLongTextConverter(t, c));
            AddEntry(typeof(short), (b, t) => b.Length, (b, t, c) => b.CreateShortTextConverter(t, c));
            AddEntry(typeof(short?), (b, t) => b.Length, (b, t, c) => b.CreateShortTextConverter(t, c));
            AddEntry(typeof(decimal), (b, t) => b.Length, (b, t, c) => b.CreateDecimalTextConverter(t, c));
            AddEntry(typeof(decimal?), (b, t) => b.Length, (b, t, c) => b.CreateDecimalTextConverter(t, c));
        }

        private IMapConverter CreateIntTextConverter(Type type, IBuilderContext context)
        {
            return new Int32TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(NumberTextParameter.Encoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(NumberTextParameter.Padding),
                Filler ?? context.GetParameter<byte>(NumberTextParameter.Filler),
                Style ?? context.GetParameter<NumberStyles>(NumberTextParameter.NumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(NumberTextParameter.Provider),
                type);
        }

        private IMapConverter CreateLongTextConverter(Type type, IBuilderContext context)
        {
            return new Int64TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(NumberTextParameter.Encoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(NumberTextParameter.Padding),
                Filler ?? context.GetParameter<byte>(NumberTextParameter.Filler),
                Style ?? context.GetParameter<NumberStyles>(NumberTextParameter.NumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(NumberTextParameter.Provider),
                type);
        }

        private IMapConverter CreateShortTextConverter(Type type, IBuilderContext context)
        {
            return new Int16TextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(NumberTextParameter.Encoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(NumberTextParameter.Padding),
                Filler ?? context.GetParameter<byte>(NumberTextParameter.Filler),
                Style ?? context.GetParameter<NumberStyles>(NumberTextParameter.NumberStyle),
                Provider ?? context.GetParameter<IFormatProvider>(NumberTextParameter.Provider),
                type);
        }

        private IMapConverter CreateDecimalTextConverter(Type type, IBuilderContext context)
        {
            return new DecimalTextConverter(
                Length,
                Format,
                Encoding ?? context.GetParameter<Encoding>(NumberTextParameter.Encoding),
                Trim ?? context.GetParameter<bool>(Parameter.Trim),
                Padding ?? context.GetParameter<Padding>(NumberTextParameter.Padding),
                Filler ?? context.GetParameter<byte>(NumberTextParameter.Filler),
                Style ?? context.GetParameter<NumberStyles>(NumberTextParameter.DecimalStyle),
                Provider ?? context.GetParameter<IFormatProvider>(NumberTextParameter.Provider),
                type);
        }
    }
}
