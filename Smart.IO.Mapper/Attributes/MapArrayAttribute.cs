namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapArrayAttribute : Attribute
    {
        private readonly int count;

        private byte? filler;

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapArrayAttribute(int count)
        {
            this.count = count;
        }

        public int CalcSize(int elementSize)
        {
            return elementSize * count;
        }

        public IByteConverter CreateArrayConverter(IMappingCreateContext context, Func<int, Array> allocator, int elementSize, IByteConverter elementConverter)
        {
            return new ArrayConverter(
                allocator,
                count,
                filler ?? context.GetParameter<byte>(Parameter.Filler),
                elementSize,
                elementConverter);
        }
    }
}
