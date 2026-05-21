namespace Smart.IO.ByteMapper.AspNetCore;

using System.Collections.Generic;

/// <summary>
/// Configuration options for ByteMapper MVC formatters and Minimal API filters.
/// </summary>
public sealed class ByteMapperFormatterOptions
{
    /// <summary>
    /// Media types handled by the ByteMapper formatters.
    /// Defaults to <c>application/octet-stream</c>.
    /// </summary>
    public IList<string> SupportedMediaTypes { get; } = ["application/octet-stream"];

    /// <summary>
    /// Internal read buffer size (bytes).  Actual buffer will be
    /// <c>Max(BufferSize, elementSize)</c>.
    /// </summary>
    public int BufferSize { get; set; } = 8192;
}
