namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BoolBinaryAttribute : Attribute
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
