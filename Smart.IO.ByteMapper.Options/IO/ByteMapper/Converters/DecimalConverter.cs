namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class DecimalConverter : IMapConverter
    {
        private readonly int length;

        private readonly byte scale;

        private readonly int groupingSize;

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly object defaultValue;

        public DecimalConverter(
            int length,
            byte scale,
            int groupingSize,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.scale = scale;
            this.groupingSize = groupingSize <= 0 ? -1 : groupingSize;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            return NumberHelper.TryParseDecimal(buffer, index, length, filler, out var result) ? result : defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                NumberHelper.FormatDecimal(buffer, index, length, (decimal)value, scale, groupingSize, padding, zerofill, filler);
            }
        }
    }
}
