namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public sealed class MapStringAttribute : AbstractPropertyAttribute
    {
        private readonly int length;

        private bool? trim;

        private Padding? padding;

        private byte? filler;

        public string Encoding { get; set; }

        public bool Trim
        {
            get => throw new NotSupportedException();
            set => trim = value;
        }

        public Padding Padding
        {
            get => throw new NotSupportedException();
            set => padding = value;
        }

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => filler = value;
        }

        public MapStringAttribute(int offset, int length)
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
            if (type == typeof(string))
            {
                return new StringConverter(
                    length,
                    System.Text.Encoding.GetEncoding(Encoding ?? context.GetParameter<string>(Parameter.TextEncoding)),
                    trim ?? context.GetParameter<bool>(Parameter.Trim),
                    padding ?? context.GetParameter<Padding>(Parameter.TextPadding),
                    filler ?? context.GetParameter<byte>(Parameter.TextFiller));
            }

            return null;
        }
    }
}
