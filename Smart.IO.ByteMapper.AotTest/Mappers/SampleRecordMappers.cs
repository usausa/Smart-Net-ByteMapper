namespace Smart.IO.ByteMapper.AotTest.Mappers;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.AotTest.Models;

internal static partial class SampleRecordMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> source, SampleRecord target);

    [ByteWriter]
    public static partial void Write(SampleRecord source, Span<byte> destination);

    [ByteReader]
    public static partial SampleRecord ReadNew(ReadOnlySpan<byte> source);

    [ByteWriter]
    public static partial byte[] WriteAlloc(SampleRecord source);
}
