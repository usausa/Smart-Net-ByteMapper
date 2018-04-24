namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public sealed class DateTimeConverterBuilder : AbstractMapConverterBuilder<DateTimeConverterBuilder>
    {
        public string Format { get; set; }

        public DateTimeKind? Kind { get; set; }

        public byte? Filler { get; set; }

        static DateTimeConverterBuilder()
        {
            AddEntry(typeof(DateTime), (b, t) => b.Format.Length, (b, t, c) => b.CreateDateTimeConverter(t, c));
            AddEntry(typeof(DateTime?), (b, t) => b.Format.Length, (b, t, c) => b.CreateDateTimeConverter(t, c));
            AddEntry(typeof(DateTimeOffset), (b, t) => b.Format.Length, (b, t, c) => b.CreateDateTimeOffsetConverter(t, c));
            AddEntry(typeof(DateTimeOffset?), (b, t) => b.Format.Length, (b, t, c) => b.CreateDateTimeOffsetConverter(t, c));
        }

        private IMapConverter CreateDateTimeConverter(Type type, IBuilderContext context)
        {
            return new DateTimeConverter(
                Format,
                Kind ?? context.GetParameter<DateTimeKind>(OptionsParameter.DateTimeKind),
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                type);
        }

        private IMapConverter CreateDateTimeOffsetConverter(Type type, IBuilderContext context)
        {
            return new DateTimeOffsetConverter(
                Format,
                Kind ?? context.GetParameter<DateTimeKind>(OptionsParameter.DateTimeKind),
                Filler ?? context.GetParameter<byte>(Parameter.Filler),
                type);
        }
    }
}
