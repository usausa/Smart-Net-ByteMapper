namespace Smart.IO.Mapper.Converters
{
    public interface IByteConverter
    {
        //int Length { get; }

        object Read(byte[] buffer, int index);

        void Write(byte[] buffer, int index, object value);
    }
}
