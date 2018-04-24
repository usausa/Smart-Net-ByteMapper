namespace Smart.IO.ByteMapper.Expressions
{
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
                .CreateMapByExpression<BinaryExpressionObject>(28, config => config
                    .ForMember(x => x.BigEndianIntValue, m => m.Binary())
                    .ForMember(x => x.LittleEndianIntValue, m => m.Binary(Endian.Little))
                    .ForMember(x => x.BigEndianLongValue, m => m.Binary())
                    .ForMember(x => x.LittleEndianLongValue, m => m.Binary(Endian.Little))
                    .ForMember(x => x.BigEndianShortValue, m => m.Binary())
                    .ForMember(x => x.LittleEndianShortValue, m => m.Binary(Endian.Little)))
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
                LittleEndianShortValue = 1
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
                    0x01, 0x00
                },
                buffer);

            // Read
            for (var i = 0; i < buffer.Length; i++)
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
        }
    }
}
