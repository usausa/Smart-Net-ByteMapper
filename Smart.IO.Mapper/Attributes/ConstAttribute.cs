namespace Smart.IO.Mapper.Attributes
{
    public sealed class ConstAttribute : PropertyAttributeBase
    {
        public byte[] Content { get; set; }

        public ConstAttribute(int offset)
            : base(offset)
        {
        }
    }
}
