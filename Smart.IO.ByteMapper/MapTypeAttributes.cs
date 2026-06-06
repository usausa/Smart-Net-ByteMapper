namespace Smart.IO.ByteMapper;

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
        Size = size;
    }
}

// Profile-dedicated counterpart of [Map]. Marks a class as a profile layout whose members are
// described by the class-level [Map...Member] attributes (the target type is referenced by name).
// Keeping it separate from [Map] avoids confusing an object's own layout with a profile layout.
// [Map] のプロファイル専用版。クラスをプロファイルレイアウトとして示し、メンバーはクラスレベルの
// [Map...Member] 属性で記述する（対象型は名前で参照）。[Map] と分けることで、オブジェクト自身の
// レイアウトとプロファイルのレイアウトの取り違えを防ぐ。
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class MapProfileAttribute : Attribute
{
    public int Size { get; }

    public bool AutoFiller { get; set; } = true;

    public bool UseDelimiter { get; set; } = true;

    public byte? NullFiller { get; set; }

    public byte[]? Delimiter { get; set; }

    public MapProfileAttribute(int size)
    {
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
        Offset = offset;
        Content = content;
    }
}
