namespace Smart.IO.ByteMapper.Expressions;

#pragma warning disable IDE0320
public sealed class MapAsciiExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByAsciiExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultTrim(true)
            .DefaultTextPadding(Padding.Right)
            .DefaultTextFiller(0x20)
            .CreateMapByExpression<AsciiExpressionObject>(8, config => config
                .ForMember(
                    x => x.StringValue,
                    m => m.Ascii(4))
                .ForMember(
                    x => x.CustomStringValue,
                    m => m.Ascii(4).Trim(false).Padding(Padding.Left).Filler((byte)'_')))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<AsciiExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new AsciiExpressionObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal("    ____"u8.ToArray(), buffer);

        // Read
        mapper.FromByte("12  __AB"u8.ToArray(), 0, obj);

        Assert.Equal("12", obj.StringValue);
        Assert.Equal("__AB", obj.CustomStringValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapAsciiExpression(-1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal sealed class AsciiExpressionObject
    {
        public string StringValue { get; set; }

        public string CustomStringValue { get; set; }
    }
}
