namespace Smart.IO.ByteMapper.AspNetCore;

/// <summary>
/// Non-generic base for <see cref="ByteMapperBinding{T}"/>.
/// Provides object-level Read/Write/Factory that allow the MVC formatters to
/// dispatch without knowing T at compile time.  Boxing is limited to one cast
/// per single-entity call; it is negligible for reference types and tolerable
/// for fixed-length value-type records.
/// </summary>
public abstract class ByteMapperBinding
{
    /// <summary>Byte size of a single serialized entity.</summary>
    public abstract int Size { get; }

    /// <summary>Creates a new, default-initialized entity instance.</summary>
    public abstract object Factory();

    /// <summary>Reads bytes into an existing entity instance.</summary>
    public abstract void Read(ReadOnlySpan<byte> source, object target);

    /// <summary>Writes an entity instance to bytes.</summary>
    public abstract void Write(object source, Span<byte> destination);
}

/// <summary>
/// Strongly-typed binding produced by the source generator for entity type
/// <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public sealed class ByteMapperBinding<T> : ByteMapperBinding
{
    public delegate void ReadDelegate(ReadOnlySpan<byte> source, T target);
    public delegate void WriteDelegate(T source, Span<byte> destination);
    public delegate T FactoryDelegate();

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

    /// <summary>Strongly-typed read.</summary>
    public void Read(ReadOnlySpan<byte> source, T target) => readDelegate(source, target);

    /// <summary>Strongly-typed write.</summary>
    public void Write(T source, Span<byte> destination) => writeDelegate(source, destination);

    /// <summary>Strongly-typed factory.</summary>
    public T Create() => factoryDelegate();

    // ---- non-generic overrides ----

    public override object Factory() => factoryDelegate()!;

    public override void Read(ReadOnlySpan<byte> source, object target) => readDelegate(source, (T)target);

    public override void Write(object source, Span<byte> destination) => writeDelegate((T)source, destination);
}
