namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

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
    }
}
