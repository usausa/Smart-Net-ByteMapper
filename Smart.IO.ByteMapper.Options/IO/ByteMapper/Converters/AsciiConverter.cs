namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Helpers;

    internal class AsciiConverter : IMapConverter
    {
        private readonly int length;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly byte filler;

        public AsciiConverter(
            int length,
            bool trim,
            Padding padding,
            byte filler)
        {
            this.length = length;
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

            return count == 0 ? string.Empty : EncodingHelper.GetAsciiString(buffer, start, count);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                BytesHelper.CopyBytes(EncodingHelper.GetAsciiBytes((string)value), buffer, index, length, padding, filler);
            }
        }
    }
}
