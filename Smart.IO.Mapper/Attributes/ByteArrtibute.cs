namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class ByteArrtibute : AbstractPropertyAttribute
    {
        private static readonly ByteConverter Converter = new ByteConverter();

        public ByteArrtibute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(PropertyInfo pi)
        {
            return 1;
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(byte))
            {
                return Converter;
            }

            throw new ByteMapperException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
