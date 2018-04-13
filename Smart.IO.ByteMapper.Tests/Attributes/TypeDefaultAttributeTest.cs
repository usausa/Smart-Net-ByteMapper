namespace Smart.IO.ByteMapper.Attributes
{
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class TypeDefaultAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseTypeDefaultAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.UTF8)
                .DefaultNumberEncoding(Encoding.UTF8)
                .DefaultDateTimeEncoding(Encoding.UTF8)
                .DefaultNumberProvider(CultureInfo.CurrentCulture)
                .DefaultDateTimeProvider(CultureInfo.CurrentCulture)
                .DefaultNumberStyle(NumberStyles.Integer)
                .DefaultDecimalStyle(NumberStyles.Integer)
                .DefaultDateTimeStyle(DateTimeStyles.None)
                .DefaultTrim(false)
                .DefaultTextPadding(Padding.Right)
                .DefaultNumberPadding(Padding.Left)
                .DefaultFiller((byte)' ')
                .DefaultTextFiller((byte)' ')
                .DefaultNumberFiller((byte)' ')
                .DefaultEndian(Endian.Big)
                .DefaultTrueValue((byte)'1')
                .DefaultFalseValue((byte)'1')
                .CreateMapByAttribute<TypeDefaultAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<TypeDefaultAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new TypeDefaultAttributeObject
            {
                IntValue = 1,
                DecimalValue = 1,
                BoolValue = true,
                StringValue = "1"
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("1_1__1Y*\r\n"), buffer);

            // Fix
            Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
            Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeNumberEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
            Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeDateTimeEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(10)]
        [TypeDelimiter(0x0D, 0x0A)]
        [TypeEncoding("ASCII")]
        [TypeNumberEncoding("ASCII")]
        [TypeDateTimeEncoding("ASCII")]
        [TypeNumberProvider(Culture.Invaliant)]
        [TypeDateTimeProvider(Culture.Invaliant)]
        [TypeNumberStyle(NumberStyles.Any)]
        [TypeDecimalStyle(NumberStyles.Any)]
        [TypeDateTimeStyle(DateTimeStyles.None)]
        [TypeTrim(true)]
        [TypeTextPadding(Padding.Left)]
        [TypeNumberPadding(Padding.Right)]
        [TypeFiller((byte)'*')]
        [TypeTextFiller((byte)'_')]
        [TypeNumberFiller((byte)'_')]
        [TypeEndian(Endian.Little)]
        [TypeTrueValue((byte)'Y')]
        [TypeFalseValue((byte)'N')]
        internal class TypeDefaultAttributeObject
        {
            [MapNumberText(0, 2)]
            public int IntValue { get; set; }

            [MapNumberText(2, 2)]
            public int DecimalValue { get; set; }

            [MapText(4, 2)]
            public string StringValue { get; set; }

            [MapBoolean(6)]
            public bool BoolValue { get; set; }
        }
    }
}
