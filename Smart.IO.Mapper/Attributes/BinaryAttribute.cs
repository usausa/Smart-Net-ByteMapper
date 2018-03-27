namespace Smart.IO.Mapper.Attributes
{
    public sealed class BinaryAttribute : PropertyAttributeBase
    {
        public Endian Endian { get; set; }

        public BinaryAttribute(int offset)
            : base(offset)
        {
        }
    }
}
