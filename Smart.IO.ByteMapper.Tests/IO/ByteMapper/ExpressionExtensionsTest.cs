namespace Smart.IO.ByteMapper;

using Xunit;

public class ExpressionExtensionsTest
{
    //--------------------------------------------------------------------------------
    // Call
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapByExpressionCall()
    {
        // Generic

        // Validation
        Assert.NotNull(new MapperFactoryConfig()
            .CreateMapByExpression<SimpleObject>(0, c => c.WithValidation(true))
            .ToMapperFactory()
            .Create<SimpleObject>());

        // Named
        Assert.NotNull(new MapperFactoryConfig()
            .CreateMapByExpression<SimpleObject>("test", 0, _ => { })
            .ToMapperFactory()
            .Create<SimpleObject>("test"));

        // Type

        // Type
        Assert.NotNull(new MapperFactoryConfig()
            .CreateMapByExpression(typeof(SimpleObject), 0, _ => { })
            .ToMapperFactory()
            .Create<SimpleObject>());

        // Type Named
        Assert.NotNull(new MapperFactoryConfig()
            .CreateMapByExpression(typeof(SimpleObject), "test", 0, _ => { })
            .ToMapperFactory()
            .Create<SimpleObject>("test"));

        // Profile

        // Generic

        // Default
        Assert.NotNull(new MapperFactoryConfig()
            .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression<SimpleObject>(0, _ => { })))
            .ToMapperFactory()
            .Create<SimpleObject>());

        // Named
        Assert.NotNull(new MapperFactoryConfig()
            .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression<SimpleObject>(0, _ => { })))
            .ToMapperFactory()
            .Create<SimpleObject>("test"));

        // Type

        // Type
        Assert.NotNull(new MapperFactoryConfig()
            .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression(typeof(SimpleObject), 0, _ => { })))
            .ToMapperFactory()
            .Create<SimpleObject>());

        // Type Named
        Assert.NotNull(new MapperFactoryConfig()
            .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression(typeof(SimpleObject), 0, _ => { })))
            .ToMapperFactory()
            .Create<SimpleObject>("test"));
    }

    //--------------------------------------------------------------------------------
    // Fix
    //--------------------------------------------------------------------------------

    [Fact]
    public void CoverageFix()
    {
        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByExpression<SimpleObject>(0, null));
        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByExpression<SimpleObject>("test", 0, null));
        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByExpression(typeof(SimpleObject), 0, null));
        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByExpression(typeof(SimpleObject), "test", 0, null));

        Assert.Throws<ArgumentNullException>(() => new AnonymousProfile(p => p.CreateMapByExpression<SimpleObject>(0, null)));
        Assert.Throws<ArgumentNullException>(() => new AnonymousProfile(p => p.CreateMapByExpression(typeof(SimpleObject), 0, null)));
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    internal class SimpleObject
    {
    }

    internal class AnonymousProfile : MapperProfile
    {
        public AnonymousProfile(Action<MapperProfile> action)
        {
            action(this);
        }
    }

    internal class NamedProfile : MapperProfile
    {
        public NamedProfile(string name, Action<MapperProfile> action)
            : base(name)
        {
            action(this);
        }
    }
}
