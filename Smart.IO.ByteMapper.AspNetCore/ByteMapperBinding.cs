namespace Smart.IO.ByteMapper.AspNetCore;

// Non-generic base for ByteMapperBinding<T>.
// Provides object-level Read/Write/Factory that allow the MVC formatters to
// dispatch without knowing T at compile time.
public abstract class ByteMapperBinding
{
    // Byte size of a single serialized entity.
    public abstract int Size { get; }

    // Creates a new, default-initialized entity instance.
    public abstract object Factory();

    // Reads bytes into an existing entity instance.
    public abstract void Read(ReadOnlySpan<byte> source, object target);

    // Writes an entity instance to bytes.
    public abstract void Write(object source, Span<byte> destination);
}

// Strongly-typed binding produced by the source generator for entity type
public sealed class ByteMapperBinding<T> : ByteMapperBinding
{
#pragma warning disable CA1711
    public delegate void ReadDelegate(ReadOnlySpan<byte> source, T target);

    public delegate void WriteDelegate(T source, Span<byte> destination);

    public delegate T FactoryDelegate();
#pragma warning restore CA1711

    private readonly ReadDelegate readDelegate;
    private readonly WriteDelegate writeDelegate;
    private readonly FactoryDelegate factoryDelegate;

    public override int Size { get; }

    public ByteMapperBinding(int size, ReadDelegate read, WriteDelegate write, FactoryDelegate factory)
    {
        Size = size;
        readDelegate = read;
        writeDelegate = write;
        factoryDelegate = factory;
    }

    // Strongly-typed read.
    public void Read(ReadOnlySpan<byte> source, T target) => readDelegate(source, target);

    // Strongly-typed write.
    public void Write(T source, Span<byte> destination) => writeDelegate(source, destination);

    /// <summary>Strongly-typed factory.</summary>
    public T Create() => factoryDelegate();

    // ---- non-generic overrides ----

    public override object Factory() => factoryDelegate()!;

    public override void Read(ReadOnlySpan<byte> source, object target) => readDelegate(source, (T)target);

    public override void Write(object source, Span<byte> destination) => writeDelegate((T)source, destination);
}
