namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    public sealed class DateTimeTextAttribute : PropertyAttributeBase
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
    }
}
