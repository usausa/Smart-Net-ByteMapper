namespace Smart.IO.ByteMapper;

using System.Globalization;
using System.Text;

using Smart.ComponentModel;
using Smart.Reflection;

public sealed class MapperFactoryConfig : IMapperFactoryConfig
{
    private readonly ComponentConfig config = new();

    private readonly Dictionary<string, object> parameters = [];

    private readonly List<IMappingFactory> factories = [];

    private readonly List<IMapperProfile> profiles = [];

    public MapperFactoryConfig()
    {
        config.Add<IDelegateFactory>(DelegateFactory.Default);

        // ReSharper disable once UseUtf8StringLiteral
#pragma warning disable IDE0230
        this.DefaultDelimiter(0x0D, 0x0A);
#pragma warning restore IDE0230
        this.DefaultEncoding(Encoding.ASCII);
        this.DefaultTrim(true);
        this.DefaultTextPadding(Padding.Right);
        this.DefaultFiller(0x20);
        this.DefaultTextFiller(0x20);
        this.DefaultEndian(Endian.Big);
        this.DefaultDateTimeKind(DateTimeKind.Unspecified);
        this.DefaultTrueValue(0x31);
        this.DefaultFalseValue(0x30);

        this.DefaultDateTimeTextEncoding(Encoding.ASCII);
        this.DefaultDateTimeTextProvider(CultureInfo.InvariantCulture);
        this.DefaultDateTimeTextStyle(DateTimeStyles.None);

        this.DefaultNumberTextEncoding(Encoding.ASCII);
        this.DefaultNumberTextProvider(CultureInfo.InvariantCulture);
        this.DefaultNumberTextNumberStyle(NumberStyles.Integer);
        this.DefaultNumberTextDecimalStyle(NumberStyles.Number);
        this.DefaultNumberTextPadding(Padding.Left);
        this.DefaultNumberTextFiller(0x20);
    }

    public MapperFactoryConfig Configure(Action<ComponentConfig> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        action(config);

        return this;
    }

    public MapperFactoryConfig AddParameter<T>(string name, T parameter)
    {
        parameters[name] = parameter;

        return this;
    }

    public MapperFactoryConfig AddMappingFactory(IMappingFactory factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        factories.Add(factory);
        return this;
    }

    public MapperFactoryConfig AddProfile(IMapperProfile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        profiles.Add(profile);
        return this;
    }

    public MapperFactoryConfig AddProfile<TProfile>()
        where TProfile : IMapperProfile, new()
    {
        profiles.Add(new TProfile());
        return this;
    }

    //--------------------------------------------------------------------------------
    // IByteMapperConfig
    //--------------------------------------------------------------------------------

    ComponentContainer IMapperFactoryConfig.ResolveComponents() => config.ToContainer();

    IDictionary<string, object> IMapperFactoryConfig.ResolveParameters() => new Dictionary<string, object>(parameters);

    IEnumerable<IMappingFactory> IMapperFactoryConfig.ResolveMappingFactories() => factories.Concat(profiles.SelectMany(static x => x.ResolveMappingFactories()));
}
