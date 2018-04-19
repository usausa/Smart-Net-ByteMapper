﻿namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class Int32Converter : IMapConverter
    {
        private readonly int length;

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int32Converter(
            int length,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            if (BytesHelper.TryParseInt32(buffer, index, length, out var result))
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

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int64Converter(
            int length,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            if (BytesHelper.TryParseInt64(buffer, index, length, out var result))
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

        private readonly Padding padding;

        private readonly bool zerofill;

        private readonly byte filler;

        private readonly Type convertEnumType;

        private readonly object defaultValue;

        public Int16Converter(
            int length,
            Padding padding,
            bool zerofill,
            byte filler,
            Type type)
        {
            this.length = length;
            this.padding = padding;
            this.zerofill = zerofill;
            this.filler = filler;
            convertEnumType = BytesHelper.GetConvertEnumType(type);
            defaultValue = type.GetDefaultValue();
        }

        public object Read(byte[] buffer, int index)
        {
            if (BytesHelper.TryParseInt16(buffer, index, length, out var result))
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