namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class GuidConverterBuilder : AbstractMapConverterBuilder<GuidConverterBuilder>
{
    public Endian? Endian { get; set; }

    static GuidConverterBuilder()
    {
        AddEntry(typeof(Guid), 16, static (b, _, c) => b.CreateGuidBinaryConverter(c));
        AddEntry(typeof(Guid?), 16, static (b, _, c) => b.CreateNullableGuidBinaryConverter(c));
    }

    private IMapConverter CreateGuidBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianGuidBinaryConverter.Default
            : LittleEndianGuidBinaryConverter.Default;
    }

    private IMapConverter CreateNullableGuidBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianNullableGuidBinaryConverter.Default
            : LittleEndianNullableGuidBinaryConverter.Default;
    }
}
