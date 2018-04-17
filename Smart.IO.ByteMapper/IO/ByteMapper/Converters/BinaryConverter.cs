namespace Smart.IO.ByteMapper.Converters
{
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
}
