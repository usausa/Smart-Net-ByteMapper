namespace Smart.IO.ByteMapper.Converters
{
    public interface IMapConverter
    {
        object Read(byte[] buffer, int index);

        void Write(byte[] buffer, int index, object value);
    }
}
