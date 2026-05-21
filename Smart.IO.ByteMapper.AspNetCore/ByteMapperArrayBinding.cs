namespace Smart.IO.ByteMapper.AspNetCore;

/// <summary>
/// Binding for array / enumerable serialisation of entity type
/// <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Element entity type.</typeparam>
public sealed class ByteMapperArrayBinding<T>
{
    public int ElementSize { get; }
    public ByteMapperBinding<T>.ReadDelegate ReadElement { get; }
    public ByteMapperBinding<T>.WriteDelegate WriteElement { get; }
    public ByteMapperBinding<T>.FactoryDelegate Factory { get; }

    public ByteMapperArrayBinding(
        int elementSize,
        ByteMapperBinding<T>.ReadDelegate readElement,
        ByteMapperBinding<T>.WriteDelegate writeElement,
        ByteMapperBinding<T>.FactoryDelegate factory)
    {
        ElementSize = elementSize;
        ReadElement = readElement;
        WriteElement = writeElement;
        Factory = factory;
    }
}
