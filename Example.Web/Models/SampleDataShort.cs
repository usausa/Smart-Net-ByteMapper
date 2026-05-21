namespace Example.Web.Models;

using Smart.IO.ByteMapper;

// Short layout: 35 bytes (33 data + 2 bytes CRLF delimiter)
// Code(0,13) + Name(13,20)
[Map(35)]
public sealed class SampleDataShort
{
    [MapText(0, 13)]
    public string Code { get; set; } = default!;

    [MapText(13, 20, CodePage = 932)]
    public string Name { get; set; } = default!;
}
