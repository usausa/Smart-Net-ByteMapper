namespace Smart.IO.ByteMapper.Converters
{
    using Smart.IO.ByteMapper.Helpers;

    internal sealed class UnicodeConverter : IMapConverter
    {
        private readonly int length;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly char filler;

        public UnicodeConverter(
            int length,
            bool trim,
            Padding padding,
            char filler)
        {
            this.length = length;
            this.trim = trim;
            this.padding = padding;
            this.filler = filler;
        }

        public object Read(byte[] buffer, int index)
        {
            return EncodingByteHelper.GetUnicodeString(buffer, index, length, trim, padding, filler);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                EncodingByteHelper.FillUnicode(buffer, index, length, filler);
            }
            else
            {
                EncodingByteHelper.CopyUnicodeBytes((string)value, buffer, index, length, padding, filler);
            }
        }
    }
}
