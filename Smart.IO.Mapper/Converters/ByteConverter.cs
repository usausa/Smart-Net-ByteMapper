namespace Smart.IO.Mapper.Converters
{
    internal sealed class MapConverter : IMapConverter
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
