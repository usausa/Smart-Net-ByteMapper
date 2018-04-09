namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Builders;

    public sealed class MapNumberTextAttribute : AbstractMapMemberAttribute
    {
        private readonly NumberTextConverterBuilder builder = new NumberTextConverterBuilder();

        public string Format
        {
            get => throw new NotSupportedException();
            set => builder.Format = value;
        }

        public int CodePage
        {
            get => throw new NotSupportedException();
            set => builder.Encoding = Encoding.GetEncoding(value);
        }

        public string EncodingName
        {
            get => throw new NotSupportedException();
            set => builder.Encoding = Encoding.GetEncoding(value);
        }

        public bool Trim
        {
            get => throw new NotSupportedException();
            set => builder.Trim = value;
        }

        public Padding Padding
        {
            get => throw new NotSupportedException();
            set => builder.Padding = value;
        }

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public NumberStyles Style
        {
            get => throw new NotSupportedException();
            set => builder.Style = value;
        }

        public Culture Culture
        {
            get => throw new NotSupportedException();
            set => builder.Provider = value.ToCultureInfo();
        }

        public MapNumberTextAttribute(int offset, int length)
            : base(offset)
        {
            builder.Length = length;
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
