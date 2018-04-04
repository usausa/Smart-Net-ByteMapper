namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapBytesAttribute : AbstractMapPropertyAttribute
    {
        private readonly int length;

        private byte? filler;

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapBytesAttribute(int offset, int length)
            : base(offset)
        {
            this.length = length;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if (type == typeof(byte[]))
            {
                return new BytesConverter(
                    length,
                    filler ?? parameters.GetParameter<byte>(Parameter.Filler));
            }

            return null;
        }
    }
}
