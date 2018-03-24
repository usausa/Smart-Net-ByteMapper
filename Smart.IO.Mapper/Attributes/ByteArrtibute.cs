namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ByteArrtibute : Attribute
    {
        public int Offset { get; }

        public ByteArrtibute(int offset)
        {
            Offset = offset;
        }
    }
}
