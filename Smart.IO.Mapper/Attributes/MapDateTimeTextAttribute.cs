namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;

    using Smart.IO.Mapper.Converters;

    public sealed class MapDateTimeTextAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private readonly string format;

        private int? codePage;

        private string encodingName;

        private byte? filler;

        private DateTimeStyles? style;

        private Culture? culture;

        public int CodePage
        {
            get => throw new NotSupportedException();
            set
            {
                codePage = value;
                encodingName = null;
            }
        }

        public string EncodingName
        {
            get => throw new NotSupportedException();
            set
            {
                encodingName = value;
                codePage = null;
            }
        }

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public DateTimeStyles Style
        {
            get => throw new NotSupportedException();
            set => style = value;
        }

        public Culture Culture
        {
            get => throw new NotSupportedException();
            set => culture = value;
        }

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
            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    filler ?? context.GetParameter<byte>(Parameter.Filler),
                    format,
                    style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTimeOffset?)))
            {
                return new DateTimeOffsetTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    filler ?? context.GetParameter<byte>(Parameter.Filler),
                    format,
                    style ?? context.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            return null;
        }
    }
}
