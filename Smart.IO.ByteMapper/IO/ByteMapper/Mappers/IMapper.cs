namespace Smart.IO.ByteMapper.Mappers;

public interface IMapper
{
    bool CanRead { get; }

    bool CanWrite { get; }

    void Read(byte[] buffer, int index, object target);

    void Write(byte[] buffer, int index, object target);
}
