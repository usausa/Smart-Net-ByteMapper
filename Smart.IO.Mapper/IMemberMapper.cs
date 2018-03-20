namespace Smart.IO.Mapper
{
    public interface IMemberMapper
    {
        void Read(byte[] buffer, int index, object target);

        void Write(byte[] buffer, int index, object target);
    }
}
