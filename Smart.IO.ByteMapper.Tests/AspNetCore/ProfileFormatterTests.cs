namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.Collections.Generic;
using System.Reflection;

using Smart.IO.ByteMapper.AspNetCore.Formatters;

// ── Dummy entity types for test isolation ────────────────────────────────────

#pragma warning disable CA1812
internal sealed class DefaultOnlyEntity
{
}

internal sealed class ProfileOnlyEntity
{
}

internal sealed class BothEntity
{
}

internal sealed class NoBindingEntity
{
}

internal sealed class ProfileMarker
{
}
#pragma warning restore CA1812

// ── Helpers ───────────────────────────────────────────────────────────────────

internal static class TestRegistryBuilder
{
    public static ByteMapperBinding<T> MakeBinding<T>()
        where T : new()
        => new(
            size: 4,
            read: static (_, _) => { },
            write: static (_, _) => { },
            factory: static () => new T());

    public static ByteMapperRegistry Build(
        IReadOnlyDictionary<(Type Type, Type? Profile), ByteMapperBinding> single)
        => new(single, new Dictionary<(Type Type, Type? Profile), object>());
}

internal static class FormatterTestExtensions
{
    private static readonly MethodInfo InputCanReadType =
        typeof(ByteMapperInputFormatter)
            .GetMethod("CanReadType", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo OutputCanWriteType =
        typeof(ByteMapperOutputFormatter)
            .GetMethod("CanWriteType", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static bool CanRead(this ByteMapperInputFormatter formatter, Type type)
        => (bool)InputCanReadType.Invoke(formatter, [type])!;

    public static bool CanWrite(this ByteMapperOutputFormatter formatter, Type? type)
        => (bool)OutputCanWriteType.Invoke(formatter, [type])!;
}

// ── HasAnyBinding tests ───────────────────────────────────────────────────────

public class HasAnyBindingTests
{
    private readonly ByteMapperRegistry registry = TestRegistryBuilder.Build(
        new Dictionary<(Type Type, Type? Profile), ByteMapperBinding>
        {
            { (typeof(DefaultOnlyEntity), null), TestRegistryBuilder.MakeBinding<DefaultOnlyEntity>() },
            { (typeof(ProfileOnlyEntity), typeof(ProfileMarker)), TestRegistryBuilder.MakeBinding<ProfileOnlyEntity>() },
            { (typeof(BothEntity), null), TestRegistryBuilder.MakeBinding<BothEntity>() },
            { (typeof(BothEntity), typeof(ProfileMarker)), TestRegistryBuilder.MakeBinding<BothEntity>() }
        });

    [Fact]
    public void WhenDefaultBindingExistsThenHasAnyBindingIsTrue()
        => Assert.True(registry.HasAnyBinding(typeof(DefaultOnlyEntity)));

    [Fact]
    public void WhenOnlyProfileBindingExistsThenHasAnyBindingIsTrue()
        => Assert.True(registry.HasAnyBinding(typeof(ProfileOnlyEntity)));

    [Fact]
    public void WhenBothBindingsExistThenHasAnyBindingIsTrue()
        => Assert.True(registry.HasAnyBinding(typeof(BothEntity)));

    [Fact]
    public void WhenNoBindingExistsThenHasAnyBindingIsFalse()
        => Assert.False(registry.HasAnyBinding(typeof(NoBindingEntity)));
}

// ── ByteMapperInputFormatter.CanReadType tests ────────────────────────────────

public class InputFormatterCanReadTypeTests
{
    private readonly ByteMapperInputFormatter formatter = new(
        TestRegistryBuilder.Build(new Dictionary<(Type Type, Type? Profile), ByteMapperBinding>
        {
            { (typeof(DefaultOnlyEntity), null), TestRegistryBuilder.MakeBinding<DefaultOnlyEntity>() },
            { (typeof(ProfileOnlyEntity), typeof(ProfileMarker)), TestRegistryBuilder.MakeBinding<ProfileOnlyEntity>() }
        }),
        new ByteMapperFormatterOptions());

    [Fact]
    public void WhenDefaultBindingExistsThenCanReadSingle()
        => Assert.True(formatter.CanRead(typeof(DefaultOnlyEntity)));

    [Fact]
    public void WhenOnlyProfileBindingExistsThenCanReadSingle()
        => Assert.True(formatter.CanRead(typeof(ProfileOnlyEntity)));

    [Fact]
    public void WhenNoBindingThenCannotReadSingle()
        => Assert.False(formatter.CanRead(typeof(NoBindingEntity)));

    [Fact]
    public void WhenProfileOnlyBindingExistsThenCanReadArray()
        => Assert.True(formatter.CanRead(typeof(ProfileOnlyEntity[])));

    [Fact]
    public void WhenNoBindingThenCannotReadArray()
        => Assert.False(formatter.CanRead(typeof(NoBindingEntity[])));

    [Fact]
    public void WhenProfileOnlyBindingExistsThenCanReadIEnumerable()
        => Assert.True(formatter.CanRead(typeof(IEnumerable<ProfileOnlyEntity>)));
}

// ── ByteMapperOutputFormatter.CanWriteType tests ──────────────────────────────

public class OutputFormatterCanWriteTypeTests
{
    private readonly ByteMapperOutputFormatter formatter = new(
        TestRegistryBuilder.Build(new Dictionary<(Type Type, Type? Profile), ByteMapperBinding>
        {
            { (typeof(DefaultOnlyEntity), null), TestRegistryBuilder.MakeBinding<DefaultOnlyEntity>() },
            { (typeof(ProfileOnlyEntity), typeof(ProfileMarker)), TestRegistryBuilder.MakeBinding<ProfileOnlyEntity>() }
        }),
        new ByteMapperFormatterOptions());

    [Fact]
    public void WhenDefaultBindingExistsThenCanWriteSingle()
        => Assert.True(formatter.CanWrite(typeof(DefaultOnlyEntity)));

    [Fact]
    public void WhenOnlyProfileBindingExistsThenCanWriteSingle()
        => Assert.True(formatter.CanWrite(typeof(ProfileOnlyEntity)));

    [Fact]
    public void WhenNoBindingThenCannotWriteSingle()
        => Assert.False(formatter.CanWrite(typeof(NoBindingEntity)));

    [Fact]
    public void WhenProfileOnlyBindingExistsThenCanWriteArray()
        => Assert.True(formatter.CanWrite(typeof(ProfileOnlyEntity[])));

    [Fact]
    public void WhenNoBindingThenCannotWriteArray()
        => Assert.False(formatter.CanWrite(typeof(NoBindingEntity[])));

    [Fact]
    public void WhenNullTypeThenCannotWrite()
        => Assert.False(formatter.CanWrite(null));
}
