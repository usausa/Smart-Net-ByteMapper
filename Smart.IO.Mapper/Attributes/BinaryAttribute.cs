namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class BinaryAttribute : AbstractPropertyAttribute
    {
        public Endian Endian { get; set; }

        public BinaryAttribute(int offset)
            : base(offset)
        {
        }

        public override bool Match(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(int) ||
                   pi.PropertyType == typeof(long) ||
                   pi.PropertyType == typeof(short);
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }
    }
}
