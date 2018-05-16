namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Converters;

    public sealed class BinaryConverterBuilder : AbstractMapConverterBuilder<BinaryConverterBuilder>
    {
        public Endian? Endian { get; set; }

        static BinaryConverterBuilder()
        {
            AddEntry(typeof(int), 4, (b, t, c) => b.CreateIntBinaryConverter(c));
            AddEntry(typeof(long), 8, (b, t, c) => b.CreateLongBinaryConverter(c));
            AddEntry(typeof(short), 2, (b, t, c) => b.CreateShortBinaryConverter(c));
            AddEntry(typeof(double), 8, (b, t, c) => b.CreateDoubleBinaryConverter(c));
            AddEntry(typeof(float), 4, (b, t, c) => b.CreateFloatBinaryConverter(c));
            AddEntry(typeof(decimal), 16, (b, t, c) => b.CreateDecimalBinaryConverter(c));
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
    }
}
