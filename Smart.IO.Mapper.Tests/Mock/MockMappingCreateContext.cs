namespace Smart.IO.Mapper.Mock
{
    using Smart.ComponentModel;

    public class MockMappingCreateContext : IMappingCreateContext
    {
        public IComponentContainer Components { get; } = new ComponentConfig().ToContainer();

        public T GetParameter<T>(string key)
        {
            return default;
        }
    }
}
