namespace Smart.IO.Mapper
{
    using Smart.ComponentModel;

    public interface IMappingCreateContext
    {
        IComponentContainer Components { get; }

        bool TryGetParameter<T>(string key, out T value);

        T GetParameter<T>(string key);
    }
}
