namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class BinaryAttribute : AbstractPropertyAttribute
    {
        public Endian? Endian { get; set; }

        public BinaryAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(int))
            {
                return 4;
            }

            if (pi.PropertyType == typeof(long))
            {
                return 8;
            }

            if (pi.PropertyType == typeof(int))
            {
                return 2;
            }

            return 0;
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

            throw new ByteMapperException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
