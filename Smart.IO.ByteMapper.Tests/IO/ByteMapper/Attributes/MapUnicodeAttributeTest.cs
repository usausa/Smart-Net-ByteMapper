namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Text;

    using Xunit;

    public class MapUnicodeAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByUnicodeAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultTrim(true)
                .DefaultTextPadding(Padding.Right)
                .DefaultUnicodeFiller(' ')
                .CreateMapByAttribute<UnicodeAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<UnicodeAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new UnicodeAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.Unicode.GetBytes("  __"), buffer);

            // Read
            mapper.FromByte(Encoding.Unicode.GetBytes("1 _A"), 0, obj);

            Assert.Equal("1", obj.StringValue);
            Assert.Equal("_A", obj.CustomStringValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapUnicodeAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.Trim);
            Assert.Throws<NotSupportedException>(() => attribute.Padding);
            Assert.Throws<NotSupportedException>(() => attribute.Filler);

            Assert.Throws<ArgumentOutOfRangeException>(() => new MapUnicodeAttribute(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapUnicodeAttribute(0, -1));
            Assert.Throws<ArgumentException>(() => new MapUnicodeAttribute(0, 1));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(8)]
        internal class UnicodeAttributeObject
        {
            [MapUnicode(0, 4)]
            public string StringValue { get; set; }

            [MapUnicode(4, 4, Trim = false, Padding = Padding.Left, Filler = '_')]
            public string CustomStringValue { get; set; }
        }
    }
}
