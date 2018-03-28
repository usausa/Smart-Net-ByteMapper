namespace Smart.IO.Mapper.Converters
{
    public sealed class BigEndianLongBinaryConverter : IByteConverter
    {
        public int Length => 8;

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetLongBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongBE(buffer, index, (long)value);
        }
    }

    public sealed class LittleEndianLongBinaryConverter : IByteConverter
    {
        public int Length => 8;

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetLongLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongLE(buffer, index, (long)value);
        }
    }
}
