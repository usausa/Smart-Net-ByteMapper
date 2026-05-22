namespace Smart.IO.ByteMapper.Benchmark;

using System;

// ---- Record definitions ----

[Map(4, UseDelimiter = false, AutoFiller = false)]
internal sealed class BinaryRecord
{
    [MapBinary<int>(0)]
    public int Value { get; set; }
}

[Map(20, UseDelimiter = false)]
internal sealed class TextRecord
{
    [MapText(0, 20)]
    public string Name { get; set; } = string.Empty;
}

[Map(1, UseDelimiter = false, AutoFiller = false)]
internal sealed class BoolRecord
{
    [MapBoolean(0)]
    public bool? Flag { get; set; }
}

[Map(40, UseDelimiter = false, AutoFiller = false)]
internal sealed class BinaryRecord10
{
    [MapBinary<int>(0)]
    public int V0 { get; set; }

    [MapBinary<int>(4)]
    public int V1 { get; set; }

    [MapBinary<int>(8)]
    public int V2 { get; set; }

    [MapBinary<int>(12)]
    public int V3 { get; set; }

    [MapBinary<int>(16)]
    public int V4 { get; set; }

    [MapBinary<int>(20)]
    public int V5 { get; set; }

    [MapBinary<int>(24)]
    public int V6 { get; set; }

    [MapBinary<int>(28)]
    public int V7 { get; set; }

    [MapBinary<int>(32)]
    public int V8 { get; set; }

    [MapBinary<int>(36)]
    public int V9 { get; set; }
}

// ---- Mapper (source-generated) ----

internal static partial class BenchmarkMappers
{
    // BinaryRecord
    [ByteReader]
    public static partial void ReadBinary(ReadOnlySpan<byte> buffer, BinaryRecord target);

    [ByteWriter]
    public static partial void WriteBinary(Span<byte> buffer, BinaryRecord source);

    // TextRecord
    [ByteReader]
    public static partial void ReadText(ReadOnlySpan<byte> buffer, TextRecord target);

    [ByteWriter]
    public static partial void WriteText(Span<byte> buffer, TextRecord source);

    // BoolRecord
    [ByteReader]
    public static partial void ReadBool(ReadOnlySpan<byte> buffer, BoolRecord target);

    [ByteWriter]
    public static partial void WriteBool(Span<byte> buffer, BoolRecord source);

    // BinaryRecord10
    [ByteReader]
    public static partial void ReadBinary10(ReadOnlySpan<byte> buffer, BinaryRecord10 target);

    [ByteWriter]
    public static partial void WriteBinary10(Span<byte> buffer, BinaryRecord10 source);
}

internal static partial class BenchmarkMappersAlloc
{
    [ByteWriter]
    public static partial byte[] WriteBinary10Alloc(BinaryRecord10 source);
}
