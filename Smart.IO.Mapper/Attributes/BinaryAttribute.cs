namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

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
    }
}
