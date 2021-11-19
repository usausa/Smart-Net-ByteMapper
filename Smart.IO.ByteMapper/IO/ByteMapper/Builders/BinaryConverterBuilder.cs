namespace Smart.IO.ByteMapper.Builders;

using System;

using Smart.IO.ByteMapper.Converters;

public sealed class BinaryConverterBuilder : AbstractMapConverterBuilder<BinaryConverterBuilder>
{
    public Endian? Endian { get; set; }

    static BinaryConverterBuilder()
    {
        AddEntry(typeof(int), 4, (b, _, c) => b.CreateIntBinaryConverter(c));
        AddEntry(typeof(long), 8, (b, _, c) => b.CreateLongBinaryConverter(c));
        AddEntry(typeof(short), 2, (b, _, c) => b.CreateShortBinaryConverter(c));
        AddEntry(typeof(double), 8, (b, _, c) => b.CreateDoubleBinaryConverter(c));
        AddEntry(typeof(float), 4, (b, _, c) => b.CreateFloatBinaryConverter(c));
        AddEntry(typeof(decimal), 16, (b, _, c) => b.CreateDecimalBinaryConverter(c));
        AddEntry(typeof(DateTime), 8, (b, _, c) => b.CreateDateTimeBinaryConverter(c));
        AddEntry(typeof(DateTimeOffset), 10, (b, _, c) => b.CreateDateTimeOffsetBinaryConverter(c));
    }

    private IMapConverter CreateIntBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianIntBinaryConverter.Default
            : LittleEndianIntBinaryConverter.Default;
    }

    private IMapConverter CreateLongBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianLongBinaryConverter.Default
            : LittleEndianLongBinaryConverter.Default;
    }

    private IMapConverter CreateShortBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianShortBinaryConverter.Default
            : LittleEndianShortBinaryConverter.Default;
    }

    private IMapConverter CreateDoubleBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianDoubleBinaryConverter.Default
            : LittleEndianDoubleBinaryConverter.Default;
    }

    private IMapConverter CreateFloatBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianFloatBinaryConverter.Default
            : LittleEndianFloatBinaryConverter.Default;
    }

    private IMapConverter CreateDecimalBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianDecimalBinaryConverter.Default
            : LittleEndianDecimalBinaryConverter.Default;
    }

    private IMapConverter CreateDateTimeBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        var kind = context.GetParameter<DateTimeKind>(Parameter.DateTimeKind);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? new BigEndianDateTimeBinaryConverter(kind)
            : new LittleEndianDateTimeBinaryConverter(kind);
    }

    private IMapConverter CreateDateTimeOffsetBinaryConverter(IBuilderContext context)
    {
        var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);
        return targetEndian == Smart.IO.ByteMapper.Endian.Big
            ? BigEndianDateTimeOffsetBinaryConverter.Default
            : LittleEndianDateTimeOffsetBinaryConverter.Default;
    }
}
