namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Xunit;

    public class MapBinaryAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBinaryAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEndian(Endian.Big)
                .CreateMapByAttribute<BinaryAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<BinaryAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new BinaryAttributeObject
            {
                BigEndianIntValue = 1,
                LittleEndianIntValue = 1,
                BigEndianLongValue = 1,
                LittleEndianLongValue = 1,
                BigEndianShortValue = 1,
                LittleEndianShortValue = 1,
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
            Assert.Equal(2, obj.BigEndianDoubleValue);
            Assert.Equal(2, obj.LittleEndianDoubleValue);
            Assert.Equal(2, obj.BigEndianFloatValue);
            Assert.Equal(2, obj.LittleEndianFloatValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapBinaryAttribute(0);

            Assert.Throws<NotSupportedException>(() => attribute.Endian);

            Assert.Throws<ArgumentOutOfRangeException>(() => new MapBinaryAttribute(-1));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(52)]
        internal class BinaryAttributeObject
        {
            [MapBinary(0)]
            public int BigEndianIntValue { get; set; }

            [MapBinary(4, Endian = Endian.Little)]
            public int LittleEndianIntValue { get; set; }

            [MapBinary(8)]
            public long BigEndianLongValue { get; set; }

            [MapBinary(16, Endian = Endian.Little)]
            public long LittleEndianLongValue { get; set; }

            [MapBinary(24)]
            public short BigEndianShortValue { get; set; }

            [MapBinary(26, Endian = Endian.Little)]
            public short LittleEndianShortValue { get; set; }

            [MapBinary(28)]
            public double BigEndianDoubleValue { get; set; }

            [MapBinary(36, Endian = Endian.Little)]
            public double LittleEndianDoubleValue { get; set; }

            [MapBinary(44)]
            public float BigEndianFloatValue { get; set; }

            [MapBinary(48, Endian = Endian.Little)]
            public float LittleEndianFloatValue { get; set; }
        }
    }
}
