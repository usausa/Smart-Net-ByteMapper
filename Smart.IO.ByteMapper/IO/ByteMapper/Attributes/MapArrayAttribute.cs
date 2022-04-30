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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Ignore")]
    public ArrayConverterBuilder GetArrayConverterBuilder() => builder;
}
