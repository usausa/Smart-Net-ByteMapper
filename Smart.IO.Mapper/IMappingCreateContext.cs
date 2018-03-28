namespace Smart.IO.Mapper
{
    public interface IMappingCreateContext
    {
        bool TryGetParameter<T>(string key, out T value);

        T GetParameterOr<T>(string key, T defaultValue);
    }
}
