namespace Smart.IO.Mapper.Mappings
{
    public interface IMapping
    {
        bool CanRead { get; }

        bool CanWrite { get; }

        void Read(byte[] buffer, int index, object target);

        void Write(byte[] buffer, int index, object target);
    }
}
