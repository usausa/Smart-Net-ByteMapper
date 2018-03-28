namespace Smart.IO.Mapper.Converters
{
    public sealed class BigEndianShortBinaryConverter : IByteConverter
    {
        public int Length => 2;

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetShortBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutShortBE(buffer, index, (short)value);
        }
    }

    public sealed class LittleEndianShortBinaryConverter : IByteConverter
    {
        public int Length => 2;

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetShortLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutShortLE(buffer, index, (short)value);
        }
    }
}
