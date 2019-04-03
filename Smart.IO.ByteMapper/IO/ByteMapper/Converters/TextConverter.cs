namespace Smart.IO.ByteMapper.Converters
{
    using System.Text;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class TextConverter : IMapConverter
    {
        private readonly int length;

        private readonly Encoding encoding;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        public TextConverter(
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
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            return encoding.GetString(buffer, start, count);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value is null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                BytesHelper.CopyBytes(encoding.GetBytes((string)value), buffer, index, length, padding, filler);
            }
        }
    }
}
