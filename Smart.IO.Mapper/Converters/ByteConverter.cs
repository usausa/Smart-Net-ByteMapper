namespace Smart.IO.Mapper.Converters
{
    public sealed class ByteConverter : IByteConverter
    {
        public object Read(byte[] buffer, int index)
        {
            return buffer[index];
        }

        public void Write(byte[] buffer, int index, object value)
        {
            buffer[index] = (byte)value;
        }
    }
}
