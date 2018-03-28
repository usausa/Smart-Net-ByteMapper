namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class NumberTextAttribute : AbstractPropertyAttribute
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public NumberStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public NumberTextAttribute(int offset)
            : base(offset)
        {
        }

        public override bool Match(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(int) ||
                   pi.PropertyType == typeof(long) ||
                   pi.PropertyType == typeof(short) ||
                   pi.PropertyType == typeof(decimal);
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            throw new System.NotImplementedException();
        }
    }
}
