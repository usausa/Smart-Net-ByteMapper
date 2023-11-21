namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Attributes;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
public class TypeMapperTest
{
    //--------------------------------------------------------------------------------
    // Generic
    //--------------------------------------------------------------------------------

    [Fact]
    public void FromByteCall()
    {
        var mapper = CreateTypeMapper<TargetObject>();
        var buffer = new byte[mapper.Size];

        // Default index
        mapper.FromByte(buffer, new TargetObject());

        // Multiple factory
        Assert.Single(mapper.FromByteMultiple(buffer, () => new TargetObject()));

        // Multiple index factory
        Assert.Single(mapper.FromByteMultiple(buffer, 0, () => new TargetObject()));

        // Multiple IEnumerable factory
        Assert.Single(mapper.FromByteMultiple(Enumerable.Repeat(buffer, 1), () => new TargetObject()));

        // Stream
        Assert.True(mapper.FromStream(new MemoryStream(buffer), new TargetObject()));

        // Stream can't read
        Assert.False(mapper.FromStream(new MemoryStream(), new TargetObject()));

        // Stream Multiple factory
        Assert.Single(mapper.FromStreamMultiple(new MemoryStream(buffer), () => new TargetObject()));
    }

    [Fact]
    public void ToByteCall()
    {
        var mapper = CreateTypeMapper<TargetObject>();
        var buffer = new byte[mapper.Size];

        // Default index
        mapper.ToByte(buffer, new TargetObject());

        // With allocate
        Assert.Equal(mapper.Size, mapper.ToByte(new TargetObject()).Length);

        // Multiple collection
        Assert.Equal(mapper.Size, mapper.ToByteMultiple(new[] { new TargetObject() }).Length);

        // Multiple not collection
        Assert.Equal(mapper.Size, mapper.ToByteMultiple(Enumerable.Repeat(new TargetObject(), 1)).Length);

        // Multiple
        mapper.ToByteMultiple(buffer, Enumerable.Repeat(new TargetObject(), 1));

        // Stream
        var ms = new MemoryStream();
        mapper.ToStream(ms, new TargetObject());
        Assert.Equal(mapper.Size, ms.Length);

        // Stream Multiple
        ms = new MemoryStream();
        mapper.ToStreamMultiple(ms, Enumerable.Repeat(new TargetObject(), 1));
        Assert.Equal(mapper.Size, ms.Length);
    }

    //--------------------------------------------------------------------------------
    // NonGeneric
    //--------------------------------------------------------------------------------

    [Fact]
    public void FromByteCallNonGeneric()
    {
        var mapper = CreateTypeMapper(typeof(TargetObject));
        var buffer = new byte[mapper.Size];

        // Default index
        mapper.FromByte(buffer, new TargetObject());

        // Multiple factory
        Assert.Single(mapper.FromByteMultiple(buffer, () => new TargetObject()));

        // Multiple index factory
        Assert.Single(mapper.FromByteMultiple(buffer, 0, () => new TargetObject()));

        // Multiple IEnumerable factory
        Assert.Single(mapper.FromByteMultiple(Enumerable.Repeat(buffer, 1), () => new TargetObject()));

        // Stream
        Assert.True(mapper.FromStream(new MemoryStream(buffer), new TargetObject()));

        // Stream can't read
        Assert.False(mapper.FromStream(new MemoryStream(), new TargetObject()));

        // Stream Multiple factory
        Assert.Single(mapper.FromStreamMultiple(new MemoryStream(buffer), () => new TargetObject()));
    }

    [Fact]
    public void ToByteCallNonGeneric()
    {
        var mapper = CreateTypeMapper(typeof(TargetObject));
        var buffer = new byte[mapper.Size];

        // Default index
        mapper.ToByte(buffer, new TargetObject());

        // With allocate
        Assert.Equal(mapper.Size, mapper.ToByte(new TargetObject()).Length);

        // Multiple collection
        Assert.Equal(mapper.Size, mapper.ToByteMultiple(new[] { new TargetObject() }).Length);

        // Multiple not collection
        Assert.Equal(mapper.Size, mapper.ToByteMultiple(Enumerable.Repeat(new TargetObject(), 1)).Length);

        // Multiple
        mapper.ToByteMultiple(buffer, Enumerable.Repeat(new TargetObject(), 1));

        // Stream
        var ms = new MemoryStream();
        mapper.ToStream(ms, new TargetObject());
        Assert.Equal(mapper.Size, ms.Length);

        // Stream Multiple
        ms = new MemoryStream();
        mapper.ToStreamMultiple(ms, Enumerable.Repeat(new TargetObject(), 1));
        Assert.Equal(mapper.Size, ms.Length);
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void TypeMapperTargetType()
    {
        Assert.Equal(typeof(TargetObject), CreateTypeMapper<TargetObject>().TargetType);
    }

    [Fact]
    public void TypeMapperNotExists()
    {
        Assert.Throws<ByteMapperException>(() => new MapperFactoryConfig().ToMapperFactory().Create<object>());
    }

    [Fact]
    public void TypeMapperWithNullProfile()
    {
        Assert.NotNull(new MapperFactoryConfig().CreateMapByAttribute<TargetObject>().ToMapperFactory().Create<TargetObject>(null));

        Assert.NotNull(new MapperFactoryConfig().CreateMapByAttribute(typeof(TargetObject)).ToMapperFactory().Create(typeof(TargetObject), null));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private static ITypeMapper<T> CreateTypeMapper<T>()
    {
        var mapperFactory = new MapperFactoryConfig()
            .CreateMapByAttribute<TargetObject>()
            .ToMapperFactory();
        return mapperFactory.Create<T>();
    }

    private static ITypeMapper CreateTypeMapper(Type type)
    {
        var mapperFactory = new MapperFactoryConfig()
            .CreateMapByAttribute<TargetObject>()
            .ToMapperFactory();
        return mapperFactory.Create(type);
    }

    [Map(4, UseDelimiter = false)]
    internal sealed class TargetObject
    {
        [MapBinary(0)]
        public int IntValue { get; set; }
    }
}
