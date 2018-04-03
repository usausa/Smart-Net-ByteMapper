﻿namespace Smart.IO.Mapper.Attributes
{
    using System.Text;

    using Xunit;

    public class MapConstAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByConstAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<ConstAttributeObject>()
                .DefaultDelimiter(new byte[] { 0x0d, 0x0a })
                .DefaultEncoding(Encoding.ASCII)
                .DefaultEndian(Endian.Big)
                .ToByteMapper();
            var mapper = byteMapper.Create<ConstAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new ConstAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("12\r\n"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(4, AutoDelimitter = true)]
        [MapConst(0, new byte[] { 0x31, 0x32 })]
        internal class ConstAttributeObject
        {
        }
    }
}
