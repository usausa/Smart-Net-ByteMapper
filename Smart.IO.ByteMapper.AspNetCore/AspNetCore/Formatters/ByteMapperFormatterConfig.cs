namespace Smart.AspNetCore.Formatters;

using System.Collections.Generic;

using Smart.IO.ByteMapper;
using Smart.Reflection;

public class ByteMapperFormatterConfig
{
    public MapperFactory MapperFactory { get; set; }

    public IDelegateFactory DelegateFactory { get; set; } = Smart.Reflection.DelegateFactory.Default;

    public IList<string> SupportedMediaTypes { get; } = new List<string>();

    public int BufferSize { get; set; } = 8192;
}
