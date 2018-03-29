namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArrayAttribute : Attribute
    {
        public int Count { get; }

        public ArrayAttribute(int count)
        {
            Count = count;
        }
    }
}
