namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapArrayAttribute : Attribute
    {
        private readonly int length;

        private byte? filler;

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapArrayAttribute(int length)
        {
            this.length = length;
        }

        public int CalcSize(int elementSize)
        {
            return elementSize * length;
        }

        public IMapConverter CreateArrayConverter(IComponentContainer components, IMappingParameter parameters, Func<int, Array> allocator, int elementSize, IMapConverter elementConverter)
        {
            return new ArrayConverter(
                allocator,
                length,
                filler ?? parameters.GetParameter<byte>(Parameter.Filler),
                elementSize,
                elementConverter);
        }
    }
}
