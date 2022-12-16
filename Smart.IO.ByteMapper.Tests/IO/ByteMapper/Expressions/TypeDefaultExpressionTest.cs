namespace Smart.IO.ByteMapper.Expressions;

using System.Globalization;
using System.Text;

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
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultEncoding(Encoding.UTF8)
            .DefaultTrim(false)
            .DefaultTextPadding(Padding.Right)
            .DefaultNumberPadding(Padding.Left)
            .DefaultZeroFill(false)
            .DefaultUseGrouping(false)
            .DefaultFiller((byte)' ')
            .DefaultTextFiller((byte)' ')
            .DefaultNumberFiller((byte)' ')
            .DefaultEndian(Endian.Big)
            .DefaultTrueValue((byte)'1')
            .DefaultFalseValue((byte)'1')
            .DefaultNumberTextEncoding(Encoding.UTF8)
            .DefaultNumberTextProvider(CultureInfo.CurrentCulture)
            .DefaultNumberTextNumberStyle(NumberStyles.Integer)
            .DefaultNumberTextDecimalStyle(NumberStyles.Integer)
            .DefaultNumberTextPadding(Padding.Left)
            .DefaultNumberTextFiller((byte)' ')
            .DefaultDateTimeTextEncoding(Encoding.UTF8)
            .DefaultDateTimeTextProvider(CultureInfo.CurrentCulture)
            .DefaultDateTimeTextStyle(DateTimeStyles.None)
            .CreateMapByExpression<TypeDefaultExpressionObject>(10, config => config
                .TypeDelimiter(0x0D, 0x0A)
                .TypeEncoding(Encoding.ASCII)
                .TypeTrim(true)
                .TypeTextPadding(Padding.Left)
                .TypeNumberPadding(Padding.Right)
                .TypeZeroFill(true)
                .TypeUseGrouping(false)
                .TypeFiller((byte)'*')
                .TypeTextFiller((byte)'_')
                .TypeNumberFiller((byte)'_')
                .TypeDateTimeKind(DateTimeKind.Unspecified)
                .TypeEndian(Endian.Little)
                .TypeTrueValue((byte)'Y')
                .TypeFalseValue((byte)'N')
                .TypeNumberTextEncoding(Encoding.ASCII)
                .TypeNumberTextProvider(CultureInfo.InvariantCulture)
                .TypeNumberTextNumberStyle(NumberStyles.Any)
                .TypeNumberTextDecimalStyle(NumberStyles.Any)
                .TypeNumberTextPadding(Padding.Right)
                .TypeNumberTextFiller((byte)'_')
                .TypeDateTimeTextEncoding(Encoding.ASCII)
                .TypeDateTimeTextProvider(CultureInfo.InvariantCulture)
                .TypeDateTimeTextStyle(DateTimeStyles.None)
                .ForMember(x => x.IntValue, 0, m => m.NumberText(2))
                .ForMember(x => x.DecimalValue, 2, m => m.NumberText(2))
                .ForMember(x => x.StringValue, 4, m => m.Text(2))
                .ForMember(x => x.BoolValue, 6, m => m.Boolean()))
            .ToMapperFactory();
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

    internal sealed class TypeDefaultExpressionObject
    {
        public int IntValue { get; set; }

        public int DecimalValue { get; set; }

        public string StringValue { get; set; }

        public bool BoolValue { get; set; }
    }
}
