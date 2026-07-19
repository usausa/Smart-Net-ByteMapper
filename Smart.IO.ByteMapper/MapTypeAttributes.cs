namespace Smart.IO.ByteMapper;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class MapAttribute : Attribute
{
    public int Size { get; }

    public bool AutoFiller { get; set; } = true;

    public bool UseDelimiter { get; set; } = true;

    // Gap auto-fill byte. Nullable types are not valid attribute argument types (CS0655), so this is
    // a plain byte; the generator treats the option as "set" only when the named argument is present.
    // ギャップ自動フィルのバイト値。Nullable 型は属性引数に使えない (CS0655) ため plain byte とし、
    // ジェネレーターは名前付き引数が指定された場合のみ「設定あり」として扱う。
    public byte NullFiller { get; set; }

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

    // See MapAttribute.NullFiller for why this is a plain byte. / plain byte の理由は MapAttribute.NullFiller を参照。
    public byte NullFiller { get; set; }

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
