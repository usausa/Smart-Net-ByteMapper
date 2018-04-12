namespace Smart.IO.ByteMapper.Expressions
{
    using System.Globalization;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class TypeDefaultExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseTypeDefaultExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.UTF8)
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
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<TypeDefaultExpressionObject>(10)
                        .TypeDelimiter(0x0D, 0x0A)
                        .TypeEncoding(Encoding.ASCII)
                        .TypeNumberProvider(CultureInfo.InvariantCulture)
                        .TypeDateTimeProvider(CultureInfo.InvariantCulture)
                        .TypeNumberStyle(NumberStyles.Any)
                        .TypeDecimalStyle(NumberStyles.Any)
                        .TypeDateTimeStyle(DateTimeStyles.None)
                        .TypeTrim(true)
                        .TypeTextPadding(Padding.Left)
                        .TypeNumberPadding(Padding.Right)
                        .TypeFiller((byte)'*')
                        .TypeTextFiller((byte)'_')
                        .TypeNumberFiller((byte)'_')
                        .TypeEndian(Endian.Little)
                        .TypeTrueValue((byte)'Y')
                        .TypeFalseValue((byte)'N')
                        .ForMember(x => x.IntValue, 0, c => c.NumberText(2))
                        .ForMember(x => x.DecimalValue, 2, c => c.NumberText(2))
                        .ForMember(x => x.StringValue, 4, c => c.Text(2))
                        .ForMember(x => x.BoolValue, 6, c => c.Boolean());
                }).ToMapperFactory();
            var mapper = mapperFactory.Create<TypeDefaultExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new TypeDefaultExpressionObject
            {
                IntValue = 1,
                DecimalValue = 1,
                BoolValue = true,
                StringValue = "1"
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("1_1__1Y*\r\n"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class TypeDefaultExpressionObject
        {
            public int IntValue { get; set; }

            public int DecimalValue { get; set; }

            public string StringValue { get; set; }

            public bool BoolValue { get; set; }
        }
    }
}