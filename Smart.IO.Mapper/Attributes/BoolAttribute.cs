namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    public sealed class BoolBinaryAttribute : AbstractPropertyAttribute
    {
        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public BoolBinaryAttribute(int offset)
            : base(offset)
        {
        }

        public override bool Match(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(bool);
        }
    }
}
