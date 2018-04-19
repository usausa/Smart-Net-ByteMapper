namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class Int32Converter : IMapConverter
    {
        private readonly int length;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int32Converter(
            int length,
            bool trim,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.trim = trim;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            if ((count > 0) && BytesHelper.TryParseInt32(buffer, start, count, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                BytesHelper.FormatInt32(buffer, index, length, (int)value, padding, zerofill);
            }
        }
    }

    internal sealed class Int64Converter : IMapConverter
    {
        private readonly int length;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int64Converter(
            int length,
            bool trim,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.trim = trim;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            if ((count > 0) && BytesHelper.TryParseInt64(buffer, start, count, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                BytesHelper.FormatInt64(buffer, index, length, (long)value, padding, zerofill);
            }
        }
    }

    internal sealed class Int16Converter : IMapConverter
    {
        private readonly int length;

        private readonly bool trim;

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int16Converter(
            int length,
            bool trim,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.trim = trim;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            var start = index;
            var count = length;
            if (trim)
            {
                BytesHelper.TrimRange(buffer, ref start, ref count, padding, filler);
            }

            if ((count > 0) && BytesHelper.TryParseInt16(buffer, start, count, out var result))
            {
                return convertEnumType != null ? Enum.ToObject(convertEnumType, result) : result;
            }

            return defaultValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                BytesHelper.FormatInt16(buffer, index, length, (short)value, padding, zerofill);
            }
        }
    }
}
