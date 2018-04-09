namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Converters;

    public class BinaryConverterBuilder : IMapConverterBuilder
    {
        public Endian? Endian { get; set; }

        public int CalcSize(IBuilderContext context, Type type)
        {
            if (type == typeof(int))
            {
                return 4;
            }

            if (type == typeof(long))
            {
                return 8;
            }

            if (type == typeof(short))
            {
                return 2;
            }

            return 0;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            var targetEndian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);

            if (type == typeof(int))
            {
                return targetEndian == Smart.IO.Mapper.Endian.Big
                    ? BigEndianIntBinaryConverter.Default
                    : LittleEndianIntBinaryConverter.Default;
            }

            if (type == typeof(long))
            {
                return targetEndian == Smart.IO.Mapper.Endian.Big
                    ? BigEndianLongBinaryConverter.Default
                    : LittleEndianLongBinaryConverter.Default;
            }

            if (type == typeof(short))
            {
                return targetEndian == Smart.IO.Mapper.Endian.Big
                    ? BigEndianShortBinaryConverter.Default
                    : LittleEndianShortBinaryConverter.Default;
            }

            return null;
        }
    }
}
