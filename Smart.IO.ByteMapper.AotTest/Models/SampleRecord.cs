namespace Smart.IO.ByteMapper.AotTest.Models;

using Smart.IO.ByteMapper;

// Layout: 40 bytes
// Code(0,8) + Name(8,20) + Qty(28,6) + Flag(34,1) + Value(35,4) + Padding(39,1)
[Map(40)]
public sealed class SampleRecord
{
    [MapText(0, 8)]
    public string Code { get; set; } = default!;

    [MapText(8, 20)]
    public string Name { get; set; } = default!;

    [MapNumberText<int>(28, 6)]
    public int Qty { get; set; }

    [MapBoolean(34)]
    public bool? Flag { get; set; }

    [MapBinary<int>(35)]
    public int Value { get; set; }
}
