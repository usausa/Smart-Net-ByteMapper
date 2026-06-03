namespace Smart.IO.ByteMapper.AspNetCore;

using System.Collections.Generic;

// Configuration options for ByteMapper MVC formatters and Minimal API filters.
public sealed class ByteMapperFormatterOptions
{
    // Default read/write buffer size (bytes) when not configured.
    public const int DefaultBufferSize = 8192;

    // Media types handled by the ByteMapper formatters. Defaults to application/octet-stream.
    public IList<string> SupportedMediaTypes { get; } = ["application/octet-stream"];

    // Internal read buffer size (bytes). Actual buffer will be Max(BufferSize, elementSize).
    public int BufferSize { get; set; } = DefaultBufferSize;
}
