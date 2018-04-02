namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class MapStringAttributeTest
    {
        private const byte Filler = 0x20;

        private const byte Filler2 = (byte)'_';

        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByStringAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<StringAttributeObject>()
                .DefaultDelimiter(null)
                .DefaultEncoding("ASCII")
                .DefaultTrim(true)
                .DefaultTextPadding(Padding.Right)
                .DefaultTextFiller(Filler)
                .ToByteMapper();
            var mapper = byteMapper.Create<StringAttributeObject>();

            var buffer = new byte[8];
            var obj = new StringAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new[] { Filler, Filler, Filler, Filler, Filler2, Filler2, Filler2, Filler2 }, buffer);

            // Read
            mapper.FromByte(Encoding.ASCII.GetBytes("12  AB__"), 0, obj);

            Assert.Equal("12", obj.StringValue);
            Assert.Equal("AB__", obj.CustomStringValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapStringAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.Trim);
            Assert.Throws<NotSupportedException>(() => attribute.Padding);
            Assert.Throws<NotSupportedException>(() => attribute.Filler);

            Assert.Null(attribute.CreateConverter(new MockMappingCreateContext(), typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(8)]
        internal class StringAttributeObject
        {
            [MapString(0, 4)]
            public string StringValue { get; set; }

            [MapString(4, 4, Encoding = "ASCII", Trim = false, Padding = Padding.Right, Filler = Filler2)]
            public string CustomStringValue { get; set; }
        }
    }
}
