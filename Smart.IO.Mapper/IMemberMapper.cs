namespace Smart.IO.Mapper
{
    // TODO 名称 ByteConverterにする？
    public interface IMemberMapper
    {
        //int Length { get; }

        object Read(byte[] buffer, int index);

        void Write(byte[] buffer, int index, object value);
    }
}
