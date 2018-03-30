namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class BinaryAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBinaryAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<BinaryAttributeObject>()
                .DefaultDelimiter(null)
                .DefaultEndian(Endian.Big)
                .ToByteMapper();
            var mapper = byteMapper.Create<BinaryAttributeObject>();

            var buffer = new byte[28];
            var obj = new BinaryAttributeObject
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
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new BinaryAttribute(0);

            Assert.Throws<NotSupportedException>(() => attribute.Endian);

            Assert.Equal(0, attribute.CalcSize(typeof(object)));
            Assert.Null(attribute.CreateConverter(new MockMappingCreateContext(), typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(28)]
        internal class BinaryAttributeObject
        {
            [Binary(0)]
            public int BigEndianIntValue { get; set; }

            [Binary(4, Endian = Endian.Little)]
            public int LittleEndianIntValue { get; set; }

            [Binary(8)]
            public long BigEndianLongValue { get; set; }

            [Binary(16, Endian = Endian.Little)]
            public long LittleEndianLongValue { get; set; }

            [Binary(24)]
            public short BigEndianShortValue { get; set; }

            [Binary(26, Endian = Endian.Little)]
            public short LittleEndianShortValue { get; set; }
        }
    }
}
