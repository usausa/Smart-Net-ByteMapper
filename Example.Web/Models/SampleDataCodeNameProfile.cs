namespace Example.Web.Models;

using Smart.IO.ByteMapper;

// Profile layout for SampleData: code and name only (33 bytes + 2 CRLF = 35 bytes)
// Reuses the same SampleData entity with a different byte layout, described entirely by class-level
// [Map...Member] attributes so the profile needs no members of its own.
// Code(0,13) + Name(13,20)
[MapProfile(35)]
[MapTextMember(nameof(SampleData.Code), 0, 13)]
[MapTextMember(nameof(SampleData.Name), 13, 20, CodePage = 932)]
public sealed class SampleDataCodeNameProfile
{
}
