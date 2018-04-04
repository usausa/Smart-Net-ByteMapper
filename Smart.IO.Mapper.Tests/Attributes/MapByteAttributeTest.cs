namespace Smart.IO.Mapper.Attributes
{
    using Xunit;

    public class MapByteAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByByteAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .MapByAttribute<ByteAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<ByteAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new ByteAttributeObject
            {
                ByteValue = 1,
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new byte[] { 0x01 }, buffer);

            // Read
            buffer[0] = 0x02;

            mapper.FromByte(buffer, 0, obj);

            Assert.Equal(2, obj.ByteValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapByteAttribute(0);

            Assert.Null(attribute.CreateConverter(null, null, typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(1)]
        internal class ByteAttributeObject
        {
            [MapByte(0)]
            public byte ByteValue { get; set; }
        }
    }
}
