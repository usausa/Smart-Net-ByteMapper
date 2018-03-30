namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class BytesAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        public byte? Filler { get; set; }

        public BytesAttribute(int offset, int length)
            : base(offset)
        {
            this.length = length;
        }

        public override int CalcSize(Type type)
        {
            return length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            if (type == typeof(byte[]))
            {
                return new BytesConverter(
                    length,
                    Filler ?? context.GetParameter<byte>(Parameter.TextFiller));
            }

            return null;
        }
    }
}
