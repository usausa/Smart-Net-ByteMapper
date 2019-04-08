namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.ByteMapper.Builders;

    public sealed class MapDateTimeTextAttribute : AbstractMemberMapAttribute
    {
        private readonly DateTimeTextConverterBuilder builder = new DateTimeTextConverterBuilder();

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

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public DateTimeStyles Style
        {
            get => throw new NotSupportedException();
            set => builder.Style = value;
        }

        public Culture Culture
        {
            get => throw new NotSupportedException();
            set => builder.Provider = value.ToCultureInfo();
        }

        public MapDateTimeTextAttribute(int offset, int length, string format)
            : base(offset)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
            builder.Format = format;
        }

        public override IMapConverterBuilder GetConverterBuilder() => builder;
    }
}
