namespace Smart.IO.Mapper.Attributes
{
    public sealed class BinaryAttribute : PropertyAttributeBase
    {
        public int Offset { get; }

        public Endian Endian { get; set; }

        public BinaryAttribute(int offset)
        {
            Offset = offset;
        }
    }
}
