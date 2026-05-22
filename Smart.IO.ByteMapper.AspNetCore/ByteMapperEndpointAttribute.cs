namespace Smart.IO.ByteMapper.AspNetCore;

using System;

// Marks a static partial class as an entry point for the ByteMapper AspNetCore source generator.
// The generator emits ByteMapperBinding<T> / ByteMapperArrayBinding<T> factory methods on the
// partial class, as well as an assembly-level bootstrap helper and the AddByteMapperFormatters
// extension method.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ByteMapperEndpointAttribute : Attribute
{
    // Whether to generate a ByteMapperArrayBinding<T> in addition to the single-entity binding. Defaults to true.
    public bool GenerateArrayBinding { get; init; } = true;
}
