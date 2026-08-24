namespace Smart.IO.ByteMapper.AspNetCore;

using System.Collections.Generic;

public sealed class ByteMapperFormatterOptions
{
    public const int DefaultBufferSize = 8192;

    public IList<string> SupportedMediaTypes { get; } = ["application/octet-stream"];

    public int BufferSize { get; set; } = DefaultBufferSize;
}
