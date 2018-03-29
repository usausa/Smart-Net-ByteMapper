namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class BinaryAttribute : AbstractPropertyAttribute
    {
        public Endian? Endian { get; set; }

        public BinaryAttribute(int offset)
            : base(offset)
        {
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            var endian = Endian ?? context.GetParameter<Endian>(Parameter.Endian);

            if (pi.PropertyType == typeof(int))
            {
                return endian == Mapper.Endian.Big
                    ? (IByteConverter)new BigEndianIntBinaryConverter()
                    : new LittleEndianIntBinaryConverter();
            }

            if (pi.PropertyType == typeof(long))
            {
                return endian == Mapper.Endian.Big
                    ? (IByteConverter)new BigEndianLongBinaryConverter()
                    : new LittleEndianLongBinaryConverter();
            }

            if (pi.PropertyType == typeof(short))
            {
                return endian == Mapper.Endian.Big
                    ? (IByteConverter)new BigEndianShortBinaryConverter()
                    : new LittleEndianShortBinaryConverter();
            }

            throw new InvalidOperationException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
