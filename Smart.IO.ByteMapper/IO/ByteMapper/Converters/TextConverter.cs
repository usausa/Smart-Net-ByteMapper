namespace Smart.IO.ByteMapper.Converters
{
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class TextConverter : IMapConverter
    {
        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        public int Length { get; }

        public TextConverter(
            int length,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler)
        {
            Length = length;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
        }

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.ReadString(buffer, index, Length, encoding, trim, padding, filler);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, Length, filler);
            }
            else
            {
                BytesHelper.WriteString((string)value, buffer, index, Length, encoding, padding, filler);
            }
        }
    }
}
