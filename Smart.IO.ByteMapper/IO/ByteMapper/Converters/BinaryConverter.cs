namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    //--------------------------------------------------------------------------------
    // Integer
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianIntBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianIntBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetIntBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntBE(buffer, index, (int)value);
        }
    }

    internal sealed class LittleEndianIntBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianIntBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetIntLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntLE(buffer, index, (int)value);
        }
    }

    //--------------------------------------------------------------------------------
    // Long
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianLongBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianLongBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetLongBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongBE(buffer, index, (long)value);
        }
    }

    internal sealed class LittleEndianLongBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianLongBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetLongLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongLE(buffer, index, (long)value);
        }
    }

    //--------------------------------------------------------------------------------
    // Short
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianShortBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianShortBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetShortBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutShortBE(buffer, index, (short)value);
        }
    }

    internal sealed class LittleEndianShortBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianShortBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetShortLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutShortLE(buffer, index, (short)value);
        }
    }

    //--------------------------------------------------------------------------------
    // Double
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianDoubleBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianDoubleBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.Int64ToDouble(ByteOrder.GetLongBE(buffer, index));
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongBE(buffer, index, BytesHelper.DoubleToInt64((double)value));
        }
    }

    internal sealed class LittleEndianDoubleBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianDoubleBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.Int64ToDouble(ByteOrder.GetLongLE(buffer, index));
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutLongLE(buffer, index, BytesHelper.DoubleToInt64((double)value));
        }
    }

    //--------------------------------------------------------------------------------
    // Float
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianFloatBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianFloatBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.Int32ToFloat(ByteOrder.GetIntBE(buffer, index));
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntBE(buffer, index, BytesHelper.FloatToInt32((float)value));
        }
    }

    internal sealed class LittleEndianFloatBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianFloatBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return BytesHelper.Int32ToFloat(ByteOrder.GetIntLE(buffer, index));
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntLE(buffer, index, BytesHelper.FloatToInt32((float)value));
        }
    }

    //--------------------------------------------------------------------------------
    // Decimal
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianDecimalBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianDecimalBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            var flag = ByteOrder.GetIntBE(buffer, index);
            var hi = ByteOrder.GetIntBE(buffer, index + 4);
            var mid = ByteOrder.GetIntBE(buffer, index + 8);
            var lo = ByteOrder.GetIntBE(buffer, index + 12);
            return DecimalHelper.FromBits(lo, mid, hi, flag);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            var bits = Decimal.GetBits((decimal)value);
            ByteOrder.PutIntBE(buffer, index, bits[3]);
            ByteOrder.PutIntBE(buffer, index + 4, bits[2]);
            ByteOrder.PutIntBE(buffer, index + 8, bits[1]);
            ByteOrder.PutIntBE(buffer, index + 12, bits[0]);
        }
    }

    internal sealed class LittleEndianDecimalBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianDecimalBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            var lo = ByteOrder.GetIntLE(buffer, index);
            var mid = ByteOrder.GetIntLE(buffer, index + 4);
            var hi = ByteOrder.GetIntLE(buffer, index + 8);
            var flag = ByteOrder.GetIntLE(buffer, index + 12);
            return DecimalHelper.FromBits(lo, mid, hi, flag);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            var bits = Decimal.GetBits((decimal)value);
            ByteOrder.PutIntLE(buffer, index, bits[0]);
            ByteOrder.PutIntLE(buffer, index + 4, bits[1]);
            ByteOrder.PutIntLE(buffer, index + 8, bits[2]);
            ByteOrder.PutIntLE(buffer, index + 12, bits[3]);
        }
    }

    //--------------------------------------------------------------------------------
    // DateTime
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianDateTimeBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianDateTimeBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            // TODO check
            //return new DateTime(ByteOrder.GetLongBE(buffer, index), kind);
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int index, object value)
        {
            //ByteOrder.PutLongBE(buffer, index, ((DateTime)value).Ticks);
            throw new NotImplementedException();
        }
    }

    internal sealed class LittleEndianDateTimeBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianDateTimeBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            // TODO check
            //return new DateTime(ByteOrder.GetLongLE(buffer, index), kind);
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int index, object value)
        {
            //ByteOrder.PutLongLE(buffer, index, ((DateTime)value).Ticks);
        }
    }

    //--------------------------------------------------------------------------------
    // DateTimeOffset
    //--------------------------------------------------------------------------------

    internal sealed class BigEndianDateTimeOffsetBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianDateTimeOffsetBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            //var tick = ByteOrder.GetLongBE(buffer, index);
            //var offset = ByteOrder.GetShortBE(buffer, index + 8);
            // TODO check 2
            //return new DateTimeOffset(new DateTime(tick, DateTimeKind.Unspecified), TimeSpan.FromMinutes(offset));
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int index, object value)
        {
            // TODO Bin +
            //var dateTime = (DateTimeOffset)value;
            //var tick = ByteOrder.PutLongBE(buffer, index,
            //ByteOrder.PutLongBE(buffer, index, ((DateTimeOffset)value).UtcTicks);
            throw new NotImplementedException();
        }
    }

    internal sealed class LittleEndianDateTimeOffsetBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianDateTimeOffsetBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            //return new DateTimeOffset(new DateTime(ByteOrder.GetLongLE(buffer, index), DateTimeKind.Utc));
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int index, object value)
        {
            //ByteOrder.PutLongLE(buffer, index, ((DateTimeOffset)value).UtcTicks);
            throw new NotImplementedException();
        }
    }
}
