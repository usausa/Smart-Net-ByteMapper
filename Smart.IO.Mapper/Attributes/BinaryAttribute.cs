namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class BinaryAttribute : AbstractPropertyAttribute
    {
        private static readonly IByteConverter BigEndianIntBinaryConverter = new BigEndianIntBinaryConverter();

        private static readonly IByteConverter LittleEndianIntBinaryConverter = new LittleEndianIntBinaryConverter();

        private static readonly IByteConverter BigEndianLongBinaryConverter = new BigEndianLongBinaryConverter();

        private static readonly IByteConverter LittleEndianLongBinaryConverter = new LittleEndianLongBinaryConverter();

        private static readonly IByteConverter BigEndianShortBinaryConverter = new BigEndianShortBinaryConverter();

        private static readonly IByteConverter LittleEndianShortBinaryConverter = new LittleEndianShortBinaryConverter();

        public Endian? Endian { get; set; }

        public BinaryAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            if (type == typeof(int))
            {
                return 4;
            }

            if (type == typeof(long))
            {
                return 8;
            }

            if (type == typeof(int))
            {
                return 2;
            }

            return 0;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            var endian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);

            if (type == typeof(int))
            {
                return endian == Mapper.Endian.Big ? BigEndianIntBinaryConverter : LittleEndianIntBinaryConverter;
            }

            if (type == typeof(long))
            {
                return endian == Mapper.Endian.Big ? BigEndianLongBinaryConverter : LittleEndianLongBinaryConverter;
            }

            if (type == typeof(short))
            {
                return endian == Mapper.Endian.Big ? BigEndianShortBinaryConverter : LittleEndianShortBinaryConverter;
            }

            return null;
        }
    }
}
