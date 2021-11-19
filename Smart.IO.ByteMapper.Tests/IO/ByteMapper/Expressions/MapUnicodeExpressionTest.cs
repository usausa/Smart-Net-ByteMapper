namespace Smart.IO.ByteMapper.Expressions;

using System;
using System.Text;

using Xunit;

public class MapUnicodeExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByUnicodeExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultTrim(true)
            .DefaultTextPadding(Padding.Right)
            .DefaultUnicodeFiller(' ')
            .CreateMapByExpression<UnicodeExpressionObject>(8, config => config
                .ForMember(
                    x => x.StringValue,
                    m => m.Unicode(4))
                .ForMember(
                    x => x.CustomStringValue,
                    m => m.Unicode(4).Trim(false).Padding(Padding.Left).Filler('_')))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<UnicodeExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new UnicodeExpressionObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(Encoding.Unicode.GetBytes("  __"), buffer);

        // Read
        mapper.FromByte(Encoding.Unicode.GetBytes("1 _A"), 0, obj);

        Assert.Equal("1", obj.StringValue);
        Assert.Equal("_A", obj.CustomStringValue);
    }

    [Fact]
    public void MapByUnicodeExpression2()
    {
        var mapperFactory = new MapperFactoryConfig()
            .UseOptionsDefault()
            .DefaultDelimiter(null)
            .DefaultTrim(true)
            .DefaultTextPadding(Padding.Right)
            .DefaultTextFiller((byte)' ')
            .CreateMapByExpression<UnicodeExpressionObject>(8, config => config
                .ForMember(
                    x => x.StringValue,
                    m => m.Unicode(4))
                .ForMember(
                    x => x.CustomStringValue,
                    m => m.Unicode(4).Trim(true).Padding(Padding.Left).Filler('_')))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<UnicodeExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new UnicodeExpressionObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(Encoding.Unicode.GetBytes("  __"), buffer);

        // Write2
        obj.StringValue = "1";
        obj.CustomStringValue = "A";
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(Encoding.Unicode.GetBytes("1 _A"), buffer);

        // Read
        mapper.FromByte(Encoding.Unicode.GetBytes("1 _A"), 0, obj);

        Assert.Equal("1", obj.StringValue);
        Assert.Equal("A", obj.CustomStringValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapUnicodeExpression(-1));
        Assert.Throws<ArgumentException>(() => new MapUnicodeExpression(1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal class UnicodeExpressionObject
    {
        public string StringValue { get; set; }

        public string CustomStringValue { get; set; }
    }
}
