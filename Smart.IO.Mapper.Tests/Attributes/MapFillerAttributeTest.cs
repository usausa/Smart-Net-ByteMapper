namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Xunit;

    public class MapFillerAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByFillerAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<FillerAttributeObject>()
                .DefaultDelimiter(null)
                .DefaultFiller(0x20)
                .ToByteMapper();
            var mapper = byteMapper.Create<FillerAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new FillerAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new byte[] { 0x20, 0x20, 0x20, 0x20, 0x30, 0x30 }, buffer);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapFillerAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.Filler);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(6, AutoFiller = true)]
        [MapFiller(0, 2)]
        [MapFiller(4, 2, Filler = 0x30)]
        internal class FillerAttributeObject
        {
        }
    }
}
