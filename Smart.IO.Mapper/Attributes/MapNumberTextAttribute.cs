namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;

    using Smart.IO.Mapper.Converters;

    public sealed class MapNumberTextAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private bool? trim;

        private Padding? padding;

        private byte? filler;

        private NumberStyles? style;

        public string Encoding { get; set; }

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

        // TODO
        public IFormatProvider Provider { get; set; }

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
                    System.Text.Encoding.GetEncoding(Encoding ?? context.GetParameter<string>(Parameter.Encoding)),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.Culture),
                    type);
            }

            if ((type == typeof(long)) || (type == typeof(long?)))
            {
                return new LongTextConverter(
                    length,
                    System.Text.Encoding.GetEncoding(Encoding ?? context.GetParameter<string>(Parameter.Encoding)),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.Culture),
                    type);
            }

            if ((type == typeof(short)) || (type == typeof(short?)))
            {
                return new ShortTextConverter(
                    length,
                    System.Text.Encoding.GetEncoding(Encoding ?? context.GetParameter<string>(Parameter.Encoding)),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.NumberStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.Culture),
                    type);
            }

            if ((type == typeof(decimal)) || (type == typeof(decimal?)))
            {
                return new ShortTextConverter(
                    length,
                    System.Text.Encoding.GetEncoding(Encoding ?? context.GetParameter<string>(Parameter.Encoding)),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                    filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                    style ?? context.GetParameter<NumberStyles>(Parameter.DecimalStyle),
                    Provider ?? context.GetParameter<IFormatProvider>(Parameter.Culture),
                    type);
            }

            return null;
        }
    }
}
