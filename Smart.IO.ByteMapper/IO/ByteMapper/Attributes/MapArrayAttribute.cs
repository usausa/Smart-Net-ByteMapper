namespace Smart.IO.ByteMapper.Attributes;

using Smart.IO.ByteMapper.Builders;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MapArrayAttribute : Attribute
{
    private readonly ArrayConverterBuilder builder = new();

    public byte Filler
    {
        get => throw new NotSupportedException();
        set => builder.Filler = value;
    }

    public MapArrayAttribute(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        builder.Length = length;
    }

#pragma warning disable CA1024
    public ArrayConverterBuilder GetArrayConverterBuilder() => builder;
#pragma warning restore CA1024
}
