namespace Smart.IO.ByteMapper.Mock
{
    using Smart.ComponentModel;

    using Smart.IO.ByteMapper.Builders;

    public class MockBuilderContext : IBuilderContext
    {
        public IComponentContainer Components { get; } = new ComponentConfig().ToContainer();

        public T GetParameter<T>(string key)
        {
            return default;
        }

        public bool TryGetParameter<T>(string key, out T value)
        {
            value = default;
            return true;
        }
    }
}
