namespace Smart.IO.ByteMapper.Expressions;

using System;

using Xunit;

public class MapBinaryExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByBinaryExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultEndian(Endian.Big)
            .CreateMapByExpression<BinaryExpressionObject>(120, config => config
                .ForMember(x => x.BigEndianIntValue, m => m.Binary())
                .ForMember(x => x.LittleEndianIntValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianLongValue, m => m.Binary())
                .ForMember(x => x.LittleEndianLongValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianShortValue, m => m.Binary())
                .ForMember(x => x.LittleEndianShortValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianDateTimeValue, m => m.Binary())
                .ForMember(x => x.LittleEndianDateTimeValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianDateTimeOffsetValue, m => m.Binary())
                .ForMember(x => x.LittleEndianDateTimeOffsetValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianDecimalValue, m => m.Binary())
                .ForMember(x => x.LittleEndianDecimalValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianDoubleValue, m => m.Binary())
                .ForMember(x => x.LittleEndianDoubleValue, m => m.Binary(Endian.Little))
                .ForMember(x => x.BigEndianFloatValue, m => m.Binary())
                .ForMember(x => x.LittleEndianFloatValue, m => m.Binary(Endian.Little)))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<BinaryExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new BinaryExpressionObject
        {
            BigEndianIntValue = 1,
            LittleEndianIntValue = 1,
            BigEndianLongValue = 1,
            LittleEndianLongValue = 1,
            BigEndianShortValue = 1,
            LittleEndianShortValue = 1,
            BigEndianDateTimeValue = new DateTime(1, DateTimeKind.Unspecified),
            LittleEndianDateTimeValue = new DateTime(1, DateTimeKind.Unspecified),
            BigEndianDateTimeOffsetValue = new DateTimeOffset(new DateTime(1, DateTimeKind.Unspecified), TimeSpan.Zero),
            LittleEndianDateTimeOffsetValue = new DateTimeOffset(new DateTime(1, DateTimeKind.Unspecified), TimeSpan.Zero),
            BigEndianDecimalValue = 1,
            LittleEndianDecimalValue = 1,
            BigEndianDoubleValue = 2,
            LittleEndianDoubleValue = 2,
            BigEndianFloatValue = 2,
            LittleEndianFloatValue = 2
        };

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(
            new byte[]
            {
                0x00, 0x00, 0x00, 0x01,
                0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x01,
                0x01, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40,
                0x40, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x40
            },
            buffer);

        // Read
        for (var i = 0; i < buffer.Length - 24; i++)
        {
            buffer[i] = (byte)(buffer[i] << 1);
        }

        mapper.FromByte(buffer, 0, obj);

        Assert.Equal(2, obj.BigEndianIntValue);
        Assert.Equal(2, obj.LittleEndianIntValue);
        Assert.Equal(2, obj.BigEndianLongValue);
        Assert.Equal(2, obj.LittleEndianLongValue);
        Assert.Equal(2, obj.BigEndianShortValue);
        Assert.Equal(2, obj.LittleEndianShortValue);
        Assert.Equal(2, obj.BigEndianDateTimeValue.Ticks);
        Assert.Equal(2, obj.LittleEndianDateTimeValue.Ticks);
        Assert.Equal(2, obj.BigEndianDateTimeOffsetValue.Ticks);
        Assert.Equal(2, obj.LittleEndianDateTimeOffsetValue.Ticks);
        Assert.Equal(2, obj.BigEndianDecimalValue);
        Assert.Equal(2, obj.LittleEndianDecimalValue);
        Assert.Equal(2, obj.BigEndianDoubleValue);
        Assert.Equal(2, obj.LittleEndianDoubleValue);
        Assert.Equal(2, obj.BigEndianFloatValue);
        Assert.Equal(2, obj.LittleEndianFloatValue);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal class BinaryExpressionObject
    {
        public int BigEndianIntValue { get; set; }

        public int LittleEndianIntValue { get; set; }

        public long BigEndianLongValue { get; set; }

        public long LittleEndianLongValue { get; set; }

        public short BigEndianShortValue { get; set; }

        public short LittleEndianShortValue { get; set; }

        public DateTime BigEndianDateTimeValue { get; set; }

        public DateTime LittleEndianDateTimeValue { get; set; }

        public DateTimeOffset BigEndianDateTimeOffsetValue { get; set; }

        public DateTimeOffset LittleEndianDateTimeOffsetValue { get; set; }

        public decimal BigEndianDecimalValue { get; set; }

        public decimal LittleEndianDecimalValue { get; set; }

        public double BigEndianDoubleValue { get; set; }

        public double LittleEndianDoubleValue { get; set; }

        public float BigEndianFloatValue { get; set; }

        public float LittleEndianFloatValue { get; set; }
    }
}
