namespace Smart.IO.Mapper.Attributes
{
    using System;

    public sealed class ByteArrtibute : PropertyAttributeBase
    {
        public int Offset { get; }

        public ByteArrtibute(int offset)
        {
            Offset = offset;
        }
    }
}
