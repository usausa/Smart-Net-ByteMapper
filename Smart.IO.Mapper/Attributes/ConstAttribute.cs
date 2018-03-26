namespace Smart.IO.Mapper.Attributes
{
    public sealed class ConstAttribute : PropertyAttributeBase
    {
        public int Offset { get; }

        public byte[] Content { get; set; }

        public ConstAttribute(int offset)
        {
            Offset = offset;
        }
    }
}
