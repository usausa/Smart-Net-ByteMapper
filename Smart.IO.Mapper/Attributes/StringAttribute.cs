namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Reflection;
    using System.Text;

    using Smart.IO.Mapper.Converters;

    public sealed class StringAttribute : AbstractPropertyAttribute
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public StringAttribute(int offset)
            : base(offset)
        {
        }

        protected override IByteConverter CreateConverter(IMappingCreateContext context, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(string))
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

            throw new InvalidOperationException(
                "Attribute does not match property. " +
                $"type=[{pi.DeclaringType.FullName}], " +
                $"property=[{pi.Name}], " +
                $"attribute=[{GetType().FullName}]");
        }
    }
}
