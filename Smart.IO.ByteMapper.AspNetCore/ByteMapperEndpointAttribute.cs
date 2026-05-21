namespace Smart.IO.ByteMapper.AspNetCore;

using System;

/// <summary>
/// Marks a <c>static partial class</c> as an entry point for the
/// ByteMapper AspNetCore source generator.  The generator emits
/// <see cref="ByteMapperBinding{T}"/> / <see cref="ByteMapperArrayBinding{T}"/>
/// factory methods on the partial class, as well as an assembly-level
/// bootstrap helper and the <c>AddByteMapperFormatters</c> extension method.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ByteMapperEndpointAttribute : Attribute
{
    /// <summary>
    /// Optional profile key.  When set, the generated binding is registered
    /// under <c>(EntityType, Key)</c> instead of <c>(EntityType, null)</c>.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Whether to generate a <see cref="ByteMapperArrayBinding{T}"/> in
    /// addition to the single-entity binding.  Defaults to <c>true</c>.
    /// </summary>
    public bool GenerateArrayBinding { get; init; } = true;
}
