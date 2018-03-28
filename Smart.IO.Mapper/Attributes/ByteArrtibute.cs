namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    using Smart.IO.Mapper.Converters;

    public sealed class ByteArrtibute : AbstractPropertyAttribute
    {
        public ByteArrtibute(int offset)
            : base(offset)
        {
        }

        public override bool Match(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(byte);
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }
    }
}
