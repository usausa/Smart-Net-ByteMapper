namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ArrayAttribute : Attribute
    {
        public string[] Profiles { get; set; }

        public int Count { get; }

        public ArrayAttribute(int count)
        {
            Count = count;
        }
    }
}
