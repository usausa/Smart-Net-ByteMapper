﻿namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Text;

    using Smart.IO.Mapper.Builders;

    public sealed class MapTextAttribute : AbstractMapMemberAttribute
    {
        private readonly TextConverterBuilder builder = new TextConverterBuilder();

        public int CodePage
        {
            get => throw new NotSupportedException();
            set => builder.Encoding = Encoding.GetEncoding(value);
        }

        public string EncodingName
        {
            get => throw new NotSupportedException();
            set => builder.Encoding = Encoding.GetEncoding(value);
        }

        public bool Trim
        {
            get => throw new NotSupportedException();
            set => builder.Trim = value;
        }

        public Padding Padding
        {
            get => throw new NotSupportedException();
            set => builder.Padding = value;
        }

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public MapTextAttribute(int offset, int length)
            : base(offset)
        {
            builder.Length = length;
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
