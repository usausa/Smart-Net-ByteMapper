namespace Smart.IO.Mapper
{
    using Smart.ComponentModel;

    public interface IMappingCreateContext
    {
        IComponentContainer Components { get; }

        bool TryGetParameter<T>(string key, out T value);

        T GetParameterOr<T>(string key, T defaultValue);
    }
}
