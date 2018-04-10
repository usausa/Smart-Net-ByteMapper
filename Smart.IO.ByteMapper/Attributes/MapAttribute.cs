namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        public int Size { get; }

        public bool AutoFiller { get; set; } = true;

        public bool UseDelimitter { get; set; } = true;

        public MapAttribute(int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Size = size;
        }
    }
}
