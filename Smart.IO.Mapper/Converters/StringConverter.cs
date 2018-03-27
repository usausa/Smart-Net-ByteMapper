namespace Smart.IO.Mapper.Converters
{
    using System.Text;

    using Smart.IO.Mapper.Helpers;

    public sealed class StringConverter : IByteConverter
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        public StringConverter(
            int length,
            Encoding encoding,
            bool trim,
            Padding padding,
            byte filler)
        {
            this.length = length;
            this.encoding = encoding;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
        }

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.ReadString(buffer, index, length, encoding, trim, padding, filler);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                buffer.Fill(index, length, filler);
            }
            else
            {
                BytesHelper.WriteString((string)value, buffer, index, length, encoding, padding, filler);
            }
        }
    }
}
