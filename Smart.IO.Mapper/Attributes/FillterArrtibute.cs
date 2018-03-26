namespace Smart.IO.Mapper.Attributes
{
    public sealed class BytesArrtibute : PropertyAttributeBase
    {
        public int Offset { get; }

        public int Length { get; set; }

        public byte? Filler { get; set; }

        public BytesArrtibute(int offset)
        {
            Offset = offset;
        }
    }
}
