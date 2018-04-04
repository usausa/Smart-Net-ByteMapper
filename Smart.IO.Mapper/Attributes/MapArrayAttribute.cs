namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

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

        public IByteConverter CreateArrayConverter(IComponentContainer components, IMappingParameter parameters, Func<int, Array> allocator, int elementSize, IByteConverter elementConverter)
        {
            return new ArrayConverter(
                allocator,
                count,
                filler ?? parameters.GetParameter<byte>(Parameter.Filler),
                elementSize,
                elementConverter);
        }
    }
}
