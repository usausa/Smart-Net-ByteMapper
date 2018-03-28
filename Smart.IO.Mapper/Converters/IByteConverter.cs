namespace Smart.IO.Mapper.Converters
{
    public interface IByteConverter
    {
        object Read(byte[] buffer, int index);

        void Write(byte[] buffer, int index, object value);
    }
}
