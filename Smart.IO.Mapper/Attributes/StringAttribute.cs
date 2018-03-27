namespace Smart.IO.Mapper.Attributes
{
    using System.Text;

    public sealed class StringAttribute : PropertyAttributeBase
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public bool Trim { get; set; }

        public Padding Padding { get; set; }

        public byte Filler { get; set; }

        public StringAttribute(int offset)
            : base(offset)
        {
        }
    }
}
