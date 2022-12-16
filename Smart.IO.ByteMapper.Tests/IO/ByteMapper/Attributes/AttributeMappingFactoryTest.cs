namespace Smart.IO.ByteMapper.Attributes;

using Xunit;

public class AttributeMappingFactoryTest
{
    //--------------------------------------------------------------------------------
    // Exception
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByAttributeIsOverlap()
    {
        Assert.Throws<ByteMapperException>(
            () => new MapperFactoryConfig().CreateMapByAttribute<OverlapObject>(true).ToMapperFactory().Create<OverlapObject>());

        new MapperFactoryConfig().CreateMapByAttribute<OverlapObject>(false).ToMapperFactory().Create<OverlapObject>();
    }

    [Fact]
    public void MapByAttributeIsUnmatched()
    {
        Assert.Throws<ByteMapperException>(
            () => new MapperFactoryConfig().CreateMapByAttribute<UnmatchedObject>(true).ToMapperFactory().Create<UnmatchedObject>());
    }

    [Fact]
    public void MapByArrayAttributeIsNoArray()
    {
        Assert.Throws<ByteMapperException>(
            () => new MapperFactoryConfig().CreateMapByAttribute<NoArrayObject>(true).ToMapperFactory().Create<NoArrayObject>());
    }

    [Fact]
    public void MapByArrayAttributeIsArrayUnmatched()
    {
        Assert.Throws<ByteMapperException>(
            () => new MapperFactoryConfig().CreateMapByAttribute<ArrayUnmatchedObject>(true).ToMapperFactory().Create<ArrayUnmatchedObject>());
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    [Map(6, UseDelimiter = false)]
    internal sealed class OverlapObject
    {
        [MapBinary(0)]
        public int IntValue1 { get; set; }

        [MapBinary(2)]
        public int IntValue2 { get; set; }
    }

    [Map(1, UseDelimiter = false)]
    internal sealed class UnmatchedObject
    {
        [MapBinary(0)]
        public string StringValue { get; set; }
    }

    [Map(4, UseDelimiter = false)]
    internal sealed class NoArrayObject
    {
        [MapArray(1)]
        [MapBinary(0)]
        public int ArrayValue { get; set; }
    }

    [Map(4, UseDelimiter = false)]
    internal sealed class ArrayUnmatchedObject
    {
        [MapArray(1)]
        [MapBinary(0)]
        public string[] ArrayValue { get; set; }
    }
}
