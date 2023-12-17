namespace Smart.IO.ByteMapper.Mock;

using Smart.ComponentModel;

using Smart.IO.ByteMapper.Builders;

public sealed class MockBuilderContext : IBuilderContext
{
    public ComponentContainer Components { get; } = new ComponentConfig().ToContainer();

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
