namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;

// Immutable registry of ByteMapperBinding and ByteMapperArrayBinding<T> instances,
// keyed by (Type entity, Type? profile). Populated once at startup by the source-generator
// produced bootstrap helper; no reflection is involved after construction.
public sealed class ByteMapperRegistry
{
    private readonly FrozenDictionary<(Type Type, Type? Profile), ByteMapperBinding> singleBindings;
    private readonly FrozenDictionary<(Type Type, Type? Profile), object> arrayBindings;

    public ByteMapperRegistry(
        IReadOnlyDictionary<(Type Type, Type? Profile), ByteMapperBinding> singleDict,
        IReadOnlyDictionary<(Type Type, Type? Profile), object> arrayDict)
    {
        singleBindings = singleDict.ToFrozenDictionary();
        arrayBindings = arrayDict.ToFrozenDictionary();
    }

    // ---- single entity ----

    public ByteMapperBinding<T>? GetBinding<T>()
        => singleBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding<T>? GetBinding<T, TProfile>()
        where TProfile : class
        => singleBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding? GetBinding(Type type, Type? profileType = null)
        => singleBindings.GetValueOrDefault((type, profileType));

    // ---- array / enumerable ----

    public ByteMapperArrayBinding<T>? GetArrayBinding<T>()
        => arrayBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public ByteMapperArrayBinding<T>? GetArrayBinding<T, TProfile>()
        where TProfile : class
        => arrayBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public object? GetArrayBinding(Type elementType, Type? profileType = null)
        => arrayBindings.GetValueOrDefault((elementType, profileType));
}
