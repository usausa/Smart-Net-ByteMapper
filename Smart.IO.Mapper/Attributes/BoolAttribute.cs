namespace Smart.IO.Mapper.Attributes
{
    using System;

    public sealed class BoolBinaryAttribute : PropertyAttributeBase
    {
        public int Offset { get; }

        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public BoolBinaryAttribute(int offset)
        {
            Offset = offset;
        }
    }
}
