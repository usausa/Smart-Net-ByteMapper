namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;

/// <summary>
/// Immutable registry of <see cref="ByteMapperBinding"/> and
/// <see cref="ByteMapperArrayBinding{T}"/> instances, keyed by
/// <c>(Type, profile)</c>.  Populated once at startup by the source-generator
/// produced bootstrap helper; no reflection is involved after construction.
/// </summary>
public sealed class ByteMapperRegistry
{
    private readonly FrozenDictionary<(Type Type, string? Profile), ByteMapperBinding> singleBindings;
    private readonly FrozenDictionary<(Type Type, string? Profile), object> arrayBindings;

    public ByteMapperRegistry(
        IReadOnlyDictionary<(Type, string?), ByteMapperBinding> single,
        IReadOnlyDictionary<(Type, string?), object> array)
    {
        singleBindings = single.ToFrozenDictionary();
        arrayBindings = array.ToFrozenDictionary();
    }

    // ---- single entity ----

    public ByteMapperBinding<T>? GetBinding<T>(string? profile = null)
        => singleBindings.TryGetValue((typeof(T), profile), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding? GetBinding(Type type, string? profile = null)
        => singleBindings.TryGetValue((type, profile), out var v) ? v : null;

    // ---- array / enumerable ----

    public ByteMapperArrayBinding<T>? GetArrayBinding<T>(string? profile = null)
        => arrayBindings.TryGetValue((typeof(T), profile), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public object? GetArrayBinding(Type elementType, string? profile = null)
        => arrayBindings.TryGetValue((elementType, profile), out var v) ? v : null;
}
