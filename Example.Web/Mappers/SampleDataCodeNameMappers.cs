namespace Example.Web.Mappers;

using Example.Web.Models;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.AspNetCore;

/// <summary>
/// Mapper for SampleData using the "code-name" profile (code and name fields only, 35 bytes).
/// Registered in the ByteMapperRegistry under key (SampleData, SampleDataCodeNameProfile).
/// </summary>
[ByteMapperEndpoint]
public static partial class SampleDataCodeNameMappers
{
    [ByteReader(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Write(SampleData source, Span<byte> destination);
}
