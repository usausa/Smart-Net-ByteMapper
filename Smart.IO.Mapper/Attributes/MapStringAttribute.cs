namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public sealed class MapStringAttribute : AbstractMapPropertyAttribute
    {
        private readonly int length;

        private int? codePage;

        private string encodingName;

        private bool? trim;

        private Padding? padding;

        private byte? filler;

        public int CodePage
        {
            get => throw new NotSupportedException();
            set
            {
                codePage = value;
                encodingName = null;
            }
        }

        public string EncodingName
        {
            get => throw new NotSupportedException();
            set
            {
                encodingName = value;
                codePage = null;
            }
        }

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

        public override IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            if (type == typeof(string))
            {
                return new StringConverter(
                    length,
                    AttributeParameterHelper.GetEncoding(parameters, codePage, encodingName),
                    trim ?? parameters.GetParameter<bool>(Parameter.Trim),
                    padding ?? parameters.GetParameter<Padding>(Parameter.TextPadding),
                    filler ?? parameters.GetParameter<byte>(Parameter.TextFiller));
            }

            return null;
        }
    }
}
