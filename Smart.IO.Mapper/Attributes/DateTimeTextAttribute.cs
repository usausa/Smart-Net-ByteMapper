namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    public sealed class DateTimeTextAttribute : AbstractPropertyAttribute
    {
        public Encoding Encoding { get; set; }

        public byte? Filler { get; set; }

        public string Format { get; set; }

        public DateTimeStyles? Style { get; set; }

        public IFormatProvider Provider { get; set; }

        public DateTimeTextAttribute(int offset)
            : base(offset)
        {
        }

        public override bool Match(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(DateTime) ||
                   pi.PropertyType == typeof(DateTimeOffset);
        }
    }
}
