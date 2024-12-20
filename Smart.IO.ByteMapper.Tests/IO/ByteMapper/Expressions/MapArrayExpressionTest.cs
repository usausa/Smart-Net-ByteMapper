namespace Smart.IO.ByteMapper.Expressions;

#pragma warning disable IDE0320
public sealed class MapArrayExpressionTest
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByArrayExpression()
    {
        var mapperFactory = new MapperFactoryConfig()
            .DefaultDelimiter(null)
            .DefaultFiller(0x00)
            .DefaultEndian(Endian.Big)
            .CreateMapByExpression<ArrayExpressionObject>(19, config => config
                .ForMember(x => x.ArrayValue, m => m.Array(3, e => e.Binary()))
                .ForMember(x => x.ByteArrayValue, m => m.Array(7, e => e.Byte()).Filler(0xFF)))
            .ToMapperFactory();
        var mapper = mapperFactory.Create<ArrayExpressionObject>();

        var buffer = new byte[mapper.Size];
        var obj = new ArrayExpressionObject();

        // Write
        mapper.ToByte(buffer, 0, obj);

#pragma warning disable IDE0055
        Assert.Equal(
            [
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
            ],
            buffer);
#pragma warning restore IDE0055

        // Write
        obj.ArrayValue = [1, 2, 3];
        obj.ByteArrayValue = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07];

        mapper.ToByte(buffer, 0, obj);

#pragma warning disable IDE0055
        Assert.Equal(
            [
                0x00, 0x00, 0x00, 0x01,
                0x00, 0x00, 0x00, 0x02,
                0x00, 0x00, 0x00, 0x03,
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07
            ],
            buffer);
#pragma warning restore IDE0055

        // Read
        for (var i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] > 0)
            {
                buffer[i]++;
            }
        }

        mapper.FromByte(buffer, 0, obj);

        Assert.Equal((int[])[2, 3, 4], obj.ArrayValue);
        Assert.Equal([0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08], obj.ByteArrayValue);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MemberConfigExpression().Array(-1, null));
        Assert.Throws<ArgumentNullException>(() => new MemberConfigExpression().Array(0, null));
        Assert.Throws<InvalidOperationException>(() => new MemberConfigExpression().Array(0, _ => { }));

        Assert.Throws<ArgumentOutOfRangeException>(() => new MapArrayExpression(-1, null));

        Assert.Throws<ArgumentNullException>(() => ((IMemberMapConfigSyntax)new ElementConfigExpression()).Map(null));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal sealed class ArrayExpressionObject
    {
        public int[] ArrayValue { get; set; }

        public byte[] ByteArrayValue { get; set; }
    }
}
