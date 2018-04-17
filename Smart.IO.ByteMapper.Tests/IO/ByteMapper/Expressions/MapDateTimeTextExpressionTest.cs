namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapDateTimeTextExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByDateTimeTextExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultTrim(true)
                .DefaultTextFiller(0x20)
                .DefaultDateTimeTextEncoding(Encoding.ASCII)
                .DefaultDateTimeTextProvider(CultureInfo.InvariantCulture)
                .DefaultDateTimeTextStyle(DateTimeStyles.None)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<DateTimeTextExpressionObject>(60)
                        .ForMember(
                            x => x.DateTimeValue,
                            m => m.DateTimeText(8, "yyyyMMdd"))
                        .ForMember(
                            x => x.NullableDateTimeValue,
                            m => m.DateTimeText(8, "yyyyMMdd"))
                        .ForMember(
                            x => x.CustomDateTimeValue,
                            m => m.DateTimeText(14, "yyyyMMddHHmmss").Encoding(Encoding.ASCII).Filler((byte)'_').Style(DateTimeStyles.None).Provider(CultureInfo.InvariantCulture))
                        .ForMember(
                            x => x.DateTimeOffsetValue,
                            m => m.DateTimeText(8, "yyyyMMdd"))
                        .ForMember(
                            x => x.NullableDateTimeOffsetValue,
                            m => m.DateTimeText(8, "yyyyMMdd"))
                        .ForMember(
                            x => x.CustomDateTimeOffsetValue,
                            m => m.DateTimeText(14, "yyyyMMddHHmmss").Encoding(Encoding.ASCII).Filler((byte)'_').Style(DateTimeStyles.None).Provider(CultureInfo.InvariantCulture));
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<DateTimeTextExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new DateTimeTextExpressionObject
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
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapDateTimeTextExpression(-1, null));
            Assert.Throws<ArgumentNullException>(() => new MapDateTimeTextExpression(0, null).Encoding(null));
            Assert.Throws<ArgumentNullException>(() => new MapDateTimeTextExpression(0, null).Provider(null));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class DateTimeTextExpressionObject
        {
            public DateTime DateTimeValue { get; set; }

            public DateTime? NullableDateTimeValue { get; set; }

            public DateTime? CustomDateTimeValue { get; set; }

            public DateTimeOffset DateTimeOffsetValue { get; set; }

            public DateTimeOffset? NullableDateTimeOffsetValue { get; set; }

            public DateTimeOffset? CustomDateTimeOffsetValue { get; set; }
        }
    }
}
