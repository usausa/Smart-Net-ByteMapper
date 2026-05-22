namespace Example;

using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.Converters;

internal static class Program
{
    public static void Main()
    {
        // Basic smoke test: BinaryConverter<int>
        var conv = new BinaryConverter<int>();
        var buf = new byte[4];
        conv.Write(buf, 12345);
        var result = conv.Read(buf);
        Console.WriteLine($"BinaryConverter<int> round-trip: {result == 12345}");

        // TextConverter
        var tc = new TextConverter(8);
        var tbuf = new byte[8];
        tc.Write(tbuf, "Hello");
        var ts = tc.Read(tbuf);
        Console.WriteLine($"TextConverter round-trip: {ts == "Hello"}");

        // Generator usage
        var record = new SampleRecord();
        var bytes = new byte[36];
        SampleMappers.Read(bytes, record);
        Console.WriteLine($"Generator Read: Id={record.Id}, Name='{record.Name}'");
    }
}

[Map(36, UseDelimiter = false)]
internal sealed class SampleRecord
{
    [MapBinary<int>(0)]
    public int Id { get; set; }

    [MapText(4, 32)]
    public string Name { get; set; } = string.Empty;
}

internal static partial class SampleMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, SampleRecord target);

    [ByteWriter]
    public static partial void Write(SampleRecord source, Span<byte> buffer);
}
