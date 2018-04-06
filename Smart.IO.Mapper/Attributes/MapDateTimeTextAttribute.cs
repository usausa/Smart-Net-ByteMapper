namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapDateTimeTextAttribute : AbstractMapMemberAttribute
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

        public override IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return new DateTimeTextConverter(
                    length,
                    format,
                    AttributeParameterHelper.GetEncoding(parameters, codePage, encodingName),
                    filler ?? parameters.GetParameter<byte>(Parameter.Filler),
                    style ?? parameters.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    AttributeParameterHelper.GetProvider(parameters, culture),
                    type);
            }

            if ((type == typeof(DateTimeOffset)) || (type == typeof(DateTimeOffset?)))
            {
                return new DateTimeOffsetTextConverter(
                    length,
                    format,
                    AttributeParameterHelper.GetEncoding(parameters, codePage, encodingName),
                    filler ?? parameters.GetParameter<byte>(Parameter.Filler),
                    style ?? parameters.GetParameter<DateTimeStyles>(Parameter.DateTimeStyle),
                    AttributeParameterHelper.GetProvider(parameters, culture),
                    type);
            }

            return null;
        }
    }
}
