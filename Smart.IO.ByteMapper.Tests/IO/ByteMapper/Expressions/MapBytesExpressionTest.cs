namespace Smart.IO.ByteMapper.Expressions;

using Xunit;

public class MapBytesExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByBytesExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller(0x30)
            .CreateMapByExpression<BytesAttributeObject>(8, config => config
                .ForMember(x => x.BytesValue, m => m.Bytes(4))
                .ForMember(x => x.CustomBytesValue, m => m.Bytes(4).Filler(0x30)))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<BytesAttributeObject>();

        var buffer = new byte[mapper.Size];
        var obj = new BytesAttributeObject
        {
            BytesValue = new byte[] { 0x01, 0x02, 0x03, 0x04 }
        };

        // Write
        mapper.ToByte(buffer, 0, obj);

        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x30, 0x30, 0x30, 0x30 }, buffer);

        // Read
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0xff;
        }

        mapper.FromByte(buffer, 0, obj);

        Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0xff }, obj.BytesValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapBytesExpression(-1));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal sealed class BytesAttributeObject
    {
        public byte[] BytesValue { get; set; }

        public byte[] CustomBytesValue { get; set; }
    }
}
