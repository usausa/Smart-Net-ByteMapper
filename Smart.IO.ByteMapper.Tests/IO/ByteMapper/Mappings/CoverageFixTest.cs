namespace Smart.IO.ByteMapper.Mappings;

using Smart.IO.ByteMapper.Mappers;

using Xunit;

public class CoverageFixTest
{
    [Fact]
    public void UnreadableMapping()
    {
        Assert.Throws<NotSupportedException>(() => new ConstantMapper(0, []).Also(x => x.Read(null, 0, null)));
        Assert.Throws<NotSupportedException>(() => new FillMapper(0, 0, 0x00).Also(x => x.Read(null, 0, null)));
    }
}
