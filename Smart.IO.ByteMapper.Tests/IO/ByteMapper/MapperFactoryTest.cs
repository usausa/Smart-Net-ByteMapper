namespace Smart.IO.ByteMapper;

public sealed class MapperFactoryTest
{
    [Fact]
    public void CoverageFix()
    {
        // Factory
        Assert.Throws<ArgumentNullException>(() => new MapperFactory(null));

        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().ToMapperFactory().Create(null));

        // Config
        var factory = new MapperFactoryConfig().Configure(config => { config.Add<IService, Service>(); }).ToMapperFactory();
        Assert.NotNull(factory.Components.Get<IService>());

        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().Configure(null));

        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().AddMappingFactory(null));
        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().AddProfile(null));

        // Profile
        new MapperFactoryConfig().AddProfile<MapperProfile>().ToMapperFactory();

        Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().AddProfile(new ExceptionProfile()));
    }

    internal interface IService
    {
        void Test();
    }

    internal sealed class Service : IService
    {
        public void Test()
        {
        }
    }

    internal sealed class ExceptionProfile : MapperProfile
    {
        public ExceptionProfile()
        {
            AddMappingFactory(null);
        }
    }
}
