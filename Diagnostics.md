# Diagnostics

## Mapping

| ID | Severity | Description | How to fix |
|---|---|---|---|
| SBM0001 | ❌ Error | `[ByteReader]` / `[ByteWriter]` method is not `static partial`, or has an implementation written | Declare the method as `static partial` without an implementation |
| SBM0002 | ❌ Error | `[ByteReader]` / `[ByteWriter]` method signature is not supported (including a struct target received by value and a nullable target) | Use one of the supported reader/writer signatures (receive a struct target by `ref`) |
| SBM0003 | ❌ Error | Target type has no `[Map]` attribute and no `Profile` is specified | Add `[Map]` to the target type, or specify `Profile` |
| SBM0004 | ❌ Error | Offset or length is negative | Give the offset and length non-negative values |
| SBM0005 | ⚠️ Warning | Two member ranges overlap in the layout | Adjust the offsets so that the ranges do not overlap |
| SBM0006 | ❌ Error | Layout extends past the size given to `[Map]` | Enlarge `Map(size)`, or shorten the layout |
| SBM0007 | ❌ Error | Member type is not supported by `[MapBinary]` | Use a type supported by `MapBinary` |
| SBM0008 | ❌ Error | Converter does not satisfy the converter contract, or its `Read` / `Write` type does not fit the property | Implement the converter contract, or use a converter for the property type |
| SBM0009 | ❌ Error | Property declared in the profile is not found in the target type | Correct the property name, or add the property to the target type |
| SBM0010 | ❌ Error | Profile type has no `[Map]` attribute | Add `[Map]` to the profile type |
| SBM0011 | ❌ Error | Return-value `Read` method needs a parameterless constructor on the target type that the mapper class can call (internal in the same assembly included), and the type must not have required members | Add a parameterless constructor (with `[SetsRequiredMembers]` when the type has required members), or read into an existing instance |
| SBM0012 | ⚠️ Warning | Member-mapping attributes are ignored because the type uses `[Map]` | Switch the type to `[MapProfile]`, or remove the member-mapping attributes |
| SBM0013 | ⚠️ Warning | Property-level mapping attributes are ignored under `[MapProfile]` | Use the `[Map...Member]` attributes instead |
| SBM0014 | ❌ Error | `[Map]` and `[MapProfile]` are both specified on the type | Specify either `[Map]` or `[MapProfile]`, not both |
| SBM0015 | ⚠️ Warning | Member size is not statically known, so overlap and size validation skip the member | Use a member type with a statically known size if validation is needed |
| SBM0016 | ❌ Error | Property cannot be assigned by the reader or read by the writer (static, inaccessible, init-only, required, obsolete as an error, or without the accessor) | Make the property an instance property with an accessible setter (reader) or getter (writer) |
| SBM0017 | ❌ Error | Mapper class name differs only in case from another class, so the generated file names collide; only the first class (in ordinal order) is generated | Rename one of the classes |

## Endpoint binding

| ID | Severity | Description | How to fix |
|---|---|---|---|
| SBM1001 | ❌ Error | `[ByteReader]` has no matching `[ByteWriter]` for the same entity and profile | Add the matching `[ByteWriter]` |
| SBM1002 | ❌ Error | Entity has no `[Map]` or `[MapProfile]` declaring a positive size | Declare a positive size with `[Map]` or `[MapProfile]` |
| SBM1003 | ❌ Error | `[ByteWriter]` has no matching `[ByteReader]` of the form `void Read(ReadOnlySpan<byte>, T)` for the same entity and profile | Add the matching `[ByteReader]` that reads into an existing instance |
| SBM1004 | ❌ Error | `[ByteMapperEndpoint]` class is not `partial`, is nested in a type that is not `partial` or is generic, is `private` / `protected` nested, or is generic or file-local | Declare the class (and its containing types) `partial`, and make a nested class `internal` or `public` |
| SBM1005 | ❌ Error | The binding cannot create the entity: it needs a non-abstract class with a parameterless constructor the endpoint class can call (`[SetsRequiredMembers]` when the type has required members), or a struct without required members | Add such a constructor |
| SBM1006 | ❌ Error | Endpoint class name differs only in case from another class, so the generated file names collide; only the first class (in ordinal order) gets its bindings | Rename one of the classes |
