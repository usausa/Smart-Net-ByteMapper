namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Text;

    using Xunit;

    public class MapDateTimeAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByDateTimeAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultTextFiller(0x20)
                .DefaultDateTimeKind(DateTimeKind.Unspecified)
                .CreateMapByAttribute<DateTimeAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<DateTimeAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new DateTimeAttributeObject
            {
                DateTimeValue = new DateTime(2000, 12, 31, 0, 0, 0),
                DateTimeOffsetValue = new DateTimeOffset(new DateTime(2000, 12, 31, 0, 0, 0))
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "20001231" +
                    "        " +
                    "______________" +
                    "20001231" +
                    "        " +
                    "______________"),
                buffer);

            // Read
            mapper.FromByte(buffer, obj);

            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "20010101" +
                    "20010101" +
                    "20001231235959" +
                    "20010101" +
                    "20010101" +
                    "20001231235959"),
                obj);

            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0), obj.DateTimeValue);
            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0), obj.NullableDateTimeValue);
            Assert.Equal(new DateTime(2000, 12, 31, 23, 59, 59), obj.CustomDateTimeValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2001, 1, 1, 0, 0, 0)), obj.DateTimeOffsetValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2001, 1, 1, 0, 0, 0)), obj.NullableDateTimeOffsetValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2000, 12, 31, 23, 59, 59)), obj.CustomDateTimeOffsetValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapDateTimeAttribute(0, string.Empty);

            Assert.Throws<NotSupportedException>(() => attribute.Filler);

            Assert.Throws<ArgumentOutOfRangeException>(() => new MapDateTimeAttribute(-1, string.Empty));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(60)]
        internal class DateTimeAttributeObject
        {
            [MapDateTime(0, "yyyyMMdd", DateTimeKind.Unspecified)]
            public DateTime DateTimeValue { get; set; }

            [MapDateTime(8, "yyyyMMdd")]
            public DateTime? NullableDateTimeValue { get; set; }

            [MapDateTime(16, "yyyyMMddHHmmss", Filler = (byte)'_')]
            public DateTime? CustomDateTimeValue { get; set; }

            [MapDateTime(30, "yyyyMMdd", DateTimeKind.Unspecified)]
            public DateTimeOffset DateTimeOffsetValue { get; set; }

            [MapDateTime(38, "yyyyMMdd")]
            public DateTimeOffset? NullableDateTimeOffsetValue { get; set; }

            [MapDateTime(46, "yyyyMMddHHmmss",  Filler = (byte)'_')]
            public DateTimeOffset? CustomDateTimeOffsetValue { get; set; }
        }
    }
}
