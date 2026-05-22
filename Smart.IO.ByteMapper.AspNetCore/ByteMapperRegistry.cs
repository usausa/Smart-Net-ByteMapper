namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;

/// <summary>
/// Immutable registry of <see cref="ByteMapperBinding"/> and
/// <see cref="ByteMapperArrayBinding{T}"/> instances, keyed by
/// <c>(Type entity, Type? profile)</c>.  Populated once at startup by the source-generator
/// produced bootstrap helper; no reflection is involved after construction.
/// </summary>
public sealed class ByteMapperRegistry
{
    private readonly FrozenDictionary<(Type Type, Type? Profile), ByteMapperBinding> singleBindings;
    private readonly FrozenDictionary<(Type Type, Type? Profile), object> arrayBindings;

    public ByteMapperRegistry(
        IReadOnlyDictionary<(Type, Type?), ByteMapperBinding> single,
        IReadOnlyDictionary<(Type, Type?), object> array)
    {
        singleBindings = single.ToFrozenDictionary();
        arrayBindings = array.ToFrozenDictionary();
    }

    // ---- single entity ----

    public ByteMapperBinding<T>? GetBinding<T>()
        => singleBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding<T>? GetBinding<T, TProfile>() where TProfile : class
        => singleBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding? GetBinding(Type type, Type? profileType = null)
        => singleBindings.TryGetValue((type, profileType), out var v) ? v : null;

    // ---- array / enumerable ----

    public ByteMapperArrayBinding<T>? GetArrayBinding<T>()
        => arrayBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public ByteMapperArrayBinding<T>? GetArrayBinding<T, TProfile>() where TProfile : class
        => arrayBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public object? GetArrayBinding(Type elementType, Type? profileType = null)
        => arrayBindings.TryGetValue((elementType, profileType), out var v) ? v : null;
}
