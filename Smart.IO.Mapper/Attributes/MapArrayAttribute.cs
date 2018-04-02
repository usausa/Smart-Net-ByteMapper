namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapArrayAttribute : Attribute
    {
        public int Count { get; }

        public MapArrayAttribute(int count)
        {
            Count = count;
        }
    }
}
