namespace Smart.IO.Mapper.Mappers
{
    public sealed class ByteMapper : IMemberMapper
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
