namespace Smart.IO.Mapper
{
    public interface IMemberMapper
    {
        void Read(byte[] buffer, object target);

        void Write(byte[] buffer, object target);
    }
}
