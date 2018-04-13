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
    }
}
