namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    public sealed class NumberTextAttribute : PropertyAttributeBase
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
    }
}
