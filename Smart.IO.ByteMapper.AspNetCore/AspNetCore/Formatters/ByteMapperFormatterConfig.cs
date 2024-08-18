namespace Smart.AspNetCore.Formatters;

using Smart.IO.ByteMapper;
using Smart.Reflection;

public sealed class ByteMapperFormatterConfig
{
    public MapperFactory MapperFactory { get; set; }

    public IDelegateFactory DelegateFactory { get; set; } = Smart.Reflection.DelegateFactory.Default;

    public IList<string> SupportedMediaTypes { get; } = [];

    public int BufferSize { get; set; } = 8192;
}
