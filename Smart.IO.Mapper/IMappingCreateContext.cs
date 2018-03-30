namespace Smart.IO.Mapper
{
    using Smart.ComponentModel;

    public interface IMappingCreateContext
    {
        IComponentContainer Components { get; }

        T GetParameter<T>(string key);
    }
}
