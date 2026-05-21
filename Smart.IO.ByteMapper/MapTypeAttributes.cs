namespace Smart.IO.ByteMapper;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class MapAttribute : Attribute
{
    public int Size { get; }

    public bool AutoFiller { get; set; } = true;

    public bool UseDelimiter { get; set; } = true;

    public byte? NullFiller { get; set; }

    public byte[]? Delimiter { get; set; }

    public MapAttribute(int size)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        Size = size;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapFillerAttribute : Attribute
{
    public int Offset { get; }

    public int Length { get; }

    public byte Filler { get; set; } = 0x20;

    public MapFillerAttribute(int offset, int length)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        Offset = offset;
        Length = length;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class MapConstantAttribute : Attribute
{
    public int Offset { get; }

    public byte[] Content { get; }

    public MapConstantAttribute(int offset, byte[] content)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        Offset = offset;
        Content = content;
    }
}
