namespace Example.Web.Mappers;

using Example.Web.Models;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.AspNetCore;

[ByteMapperEndpoint]
public static partial class SampleDataMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter]
    public static partial void Write(SampleData source, Span<byte> destination);
}

[ByteMapperEndpoint]
public static partial class SampleDataShortMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> source, SampleDataShort target);

    [ByteWriter]
    public static partial void Write(SampleDataShort source, Span<byte> destination);
}
