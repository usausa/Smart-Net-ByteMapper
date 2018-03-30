namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class StringAttribute : AbstractPropertyAttribute
    {
        public int Length { get; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public StringAttribute(int offset, int length)
            : base(offset)
        {
            Length = length;
        }

        public override int CalcSize(Type type)
        {
            return Length;
        }

        public override IByteConverter CreateConverter(IMappingCreateContext context, Type type)
        {
            if (type == typeof(string))
            {
                var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.TextEncoding);
                var trim = Trim ?? context.GetParameter<bool>(Parameter.Trim);
                var padding = Padding ?? context.GetParameter<Padding>(Parameter.TextPadding);
                var filler = Filler ?? context.GetParameter<byte>(Parameter.TextFiller);
                return new StringConverter(
                    Length,
                    encoding,
                    trim,
                    padding,
                    filler);
            }

            return null;
        }
    }
}
