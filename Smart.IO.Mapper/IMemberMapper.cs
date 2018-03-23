namespace Smart.IO.Mapper
{
    public interface IMemberMapper
    {
        int Length { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        void Read(byte[] buffer, int index, object target);

        void Write(byte[] buffer, int index, object target);
    }
}
