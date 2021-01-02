namespace Smart.IO.ByteMapper.Converters
{
    internal sealed class ByteConverter : IMapConverter
    {
        public static ByteConverter Default { get; } = new();

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
