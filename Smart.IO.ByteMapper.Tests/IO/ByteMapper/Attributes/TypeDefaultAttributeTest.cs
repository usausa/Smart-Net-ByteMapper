namespace Smart.IO.ByteMapper.Attributes;

using System.Globalization;
using System.Text;

public sealed class TypeDefaultAttributeTest
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
            .DefaultTrim(false)
            .DefaultTextPadding(Padding.Right)
            .DefaultFiller((byte)' ')
            .DefaultTextFiller((byte)' ')
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
            .DefaultUnicodeFiller(' ')
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

        Assert.Equal("1_1__1Y*\r\n"u8.ToArray(), buffer);

        // Fix
        Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
        Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeNumberTextEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
        Assert.Equal(Encoding.ASCII.CodePage, ((Encoding)new TypeDateTimeTextEncodingAttribute(Encoding.ASCII.CodePage).Value).CodePage);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(10)]
    [TypeDelimiter(0x0D, 0x0A)]
    [TypeEncoding("ASCII")]
    [TypeTrim(true)]
    [TypeTextPadding(Padding.Left)]
    [TypeFiller((byte)'*')]
    [TypeTextFiller((byte)'_')]
    [TypeEndian(Endian.Little)]
    [TypeDateTimeKind(DateTimeKind.Unspecified)]
    [TypeTrueValue((byte)'Y')]
    [TypeFalseValue((byte)'N')]
    [TypeNumberTextEncoding("ASCII")]
    [TypeNumberTextProvider(Culture.Invariant)]
    [TypeNumberTextNumberStyle(NumberStyles.Any)]
    [TypeNumberTextDecimalStyle(NumberStyles.Any)]
    [TypeNumberTextPadding(Padding.Right)]
    [TypeNumberTextFiller((byte)'_')]
    [TypeDateTimeTextEncoding("ASCII")]
    [TypeDateTimeTextProvider(Culture.Invariant)]
    [TypeDateTimeTextStyle(DateTimeStyles.None)]
    [TypeNumberPadding(Padding.Right)]
    [TypeNumberFiller((byte)'_')]
    [TypeZeroFill(false)]
    [TypeUseGrouping(false)]
    [TypeUnicodeFiller(' ')]
    internal sealed class TypeDefaultAttributeObject
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
