namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        public int Size { get; }

        public string[] Profiles { get; set; } = { Profile.Default };

        public string[] FillTargets { get; set; } = { Profile.Default };

        public string[] DelimitterTargets { get; set; } = { Profile.Default };

        public MapAttribute(int size)
        {
            Size = size;
        }
    }
}
