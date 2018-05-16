namespace Smart.IO.ByteMapper.Builders
{
    using Smart.ComponentModel;

    public interface IBuilderContext
    {
        IComponentContainer Components { get; }

        T GetParameter<T>(string key);

        bool TryGetParameter<T>(string key, out T value);
    }
}
