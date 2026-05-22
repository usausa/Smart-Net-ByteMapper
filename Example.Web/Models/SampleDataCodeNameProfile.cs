namespace Example.Web.Models;

using Smart.IO.ByteMapper;

// Profile layout for SampleData: code and name only (33 bytes + 2 CRLF = 35 bytes)
// Reuses the same SampleData entity with a different byte layout.
// Code(0,13) + Name(13,20)
[Map(35)]
public sealed class SampleDataCodeNameProfile
{
    [MapText(0, 13)]
    public string Code { get; set; } = default!;

    [MapText(13, 20, CodePage = 932)]
    public string Name { get; set; } = default!;
}
