namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        public int Size { get; }

        public string[] FillProfiles { get; set; } = { string.Empty };

        public MapAttribute(int size)
        {
            Size = size;
        }
    }
}
