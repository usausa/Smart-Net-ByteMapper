namespace Smart.IO.ByteMapper.Converters
{
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
            throw new System.NotImplementedException();
        }

        public void Write(byte[] buffer, int index, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
