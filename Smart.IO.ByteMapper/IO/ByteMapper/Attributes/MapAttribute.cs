namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        private byte? nullFiller;

        public int Size { get; }

        public bool HasNullFiller => nullFiller.HasValue;

        public byte NullFiller
        {
            get => nullFiller ?? 0;
            set => nullFiller = value;
        }

        public bool AutoFiller { get; set; } = true;

        public bool UseDelimiter { get; set; } = true;

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
