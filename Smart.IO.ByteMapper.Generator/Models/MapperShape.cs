namespace Smart.IO.ByteMapper.Generator.Models;

internal enum MapperShape
{
    InPlace,     // void Read(ReadOnlySpan<byte> buffer, T target)
    NewInstance,  // T Read(ReadOnlySpan<byte> buffer)
    WriteSpan,    // void Write(T source, Span<byte> buffer)
    WriteAlloc // byte[] Write(T source)
}
