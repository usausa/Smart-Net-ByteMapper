namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;

    using Smart.IO.Mapper.Converters;

    public sealed class MapNumberTextAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private int? codePage;

        private string encodingName;

        private bool? trim;

        private Padding? padding;

        private byte? filler;

        private NumberStyles? style;

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

        public bool Trim
        {
            get => throw new NotSupportedException();
            set => trim = value;
        }

        public Padding Padding
        {
            get => throw new NotSupportedException();
            set => padding = value;
        }

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public NumberStyles Style
        {
            get => throw new NotSupportedException();
            set => style = value;
        }

        public Culture Culture
        {
            get => throw new NotSupportedException();
            set => culture = value;
        }

        public MapNumberTextAttribute(int offset, int length)
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
            if ((type == typeof(int)) || (type == typeof(int?)))
            {
                return new IntTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            if ((type == typeof(long)) || (type == typeof(long?)))
            {
                return new LongTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            if ((type == typeof(short)) || (type == typeof(short?)))
            {
                return new ShortTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            if ((type == typeof(decimal)) || (type == typeof(decimal?)))
            {
                return new DecimalTextConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(context, codePage, encodingName),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.DecimalStyle),
                    AttributeParameterHelper.GetProvider(context, culture),
                    type);
            }

            return null;
        }
    }
}
