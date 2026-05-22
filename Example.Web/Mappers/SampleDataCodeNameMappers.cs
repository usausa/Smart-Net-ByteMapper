namespace Example.Web.Mappers;

using Example.Web.Models;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.AspNetCore;

// "code-name" プロファイル（code と name のみ、レコード 35 バイト）で SampleData をマッピングします。
[ByteMapperEndpoint]
public static partial class SampleDataCodeNameMappers
{
    [ByteReader(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Write(Span<byte> destination, SampleData source);
}
