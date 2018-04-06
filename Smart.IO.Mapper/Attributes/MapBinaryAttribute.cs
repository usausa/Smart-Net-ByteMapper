﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapBinaryAttribute : AbstractMapMemberAttribute
    {
        private static readonly IMapConverter BigEndianIntBinaryConverter = new BigEndianIntBinaryConverter();

        private static readonly IMapConverter LittleEndianIntBinaryConverter = new LittleEndianIntBinaryConverter();

        private static readonly IMapConverter BigEndianLongBinaryConverter = new BigEndianLongBinaryConverter();

        private static readonly IMapConverter LittleEndianLongBinaryConverter = new LittleEndianLongBinaryConverter();

        private static readonly IMapConverter BigEndianShortBinaryConverter = new BigEndianShortBinaryConverter();

        private static readonly IMapConverter LittleEndianShortBinaryConverter = new LittleEndianShortBinaryConverter();

        private Endian? endian;

        public Endian Endian
        {
            get => throw new NotSupportedException();
            set => endian = value;
        }

        public MapBinaryAttribute(int offset)
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

            if (type == typeof(short))
            {
                return 2;
            }

            return 0;
        }

        public override IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            var targetEndian = endian ?? parameters.GetParameter<Endian>(Parameter.Endian);

            if (type == typeof(int))
            {
                return targetEndian == Endian.Big ? BigEndianIntBinaryConverter : LittleEndianIntBinaryConverter;
            }

            if (type == typeof(long))
            {
                return targetEndian == Endian.Big ? BigEndianLongBinaryConverter : LittleEndianLongBinaryConverter;
            }

            if (type == typeof(short))
            {
                return targetEndian == Endian.Big ? BigEndianShortBinaryConverter : LittleEndianShortBinaryConverter;
            }

            return null;
        }
    }
}
