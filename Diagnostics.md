# Diagnostics

## Mapping

| ID | Severity | Description | How to fix |
|---|---|---|---|
| SBM0001 | ❌ Error | `[ByteReader]` / `[ByteWriter]` method is not `static partial` | Declare the method as `static partial` |
| SBM0002 | ❌ Error | `[ByteReader]` / `[ByteWriter]` method signature is not supported | Use one of the supported reader/writer signatures |
| SBM0003 | ❌ Error | Target type has no `[Map]` attribute and no `Profile` is specified | Add `[Map]` to the target type, or specify `Profile` |
| SBM0004 | ❌ Error | Offset or length is negative | Give the offset and length non-negative values |
| SBM0005 | ⚠️ Warning | Two member ranges overlap in the layout | Adjust the offsets so that the ranges do not overlap |
| SBM0006 | ❌ Error | Layout extends past the size given to `[Map]` | Enlarge `Map(size)`, or shorten the layout |
| SBM0007 | ❌ Error | Member type is not supported by `[MapBinary]` | Use a type supported by `MapBinary` |
| SBM0008 | ❌ Error | Custom converter does not satisfy the converter contract | Implement the converter contract on the custom converter |
| SBM0009 | ❌ Error | Property declared in the profile is not found in the target type | Correct the property name, or add the property to the target type |
| SBM0010 | ❌ Error | Profile type has no `[Map]` attribute | Add `[Map]` to the profile type |
| SBM0011 | ❌ Error | Return-value `Read` method needs a public parameterless constructor on the target type | Add a public parameterless constructor to the target type |
| SBM0012 | ⚠️ Warning | Member-mapping attributes are ignored because the type uses `[Map]` | Switch the type to `[MapProfile]`, or remove the member-mapping attributes |
| SBM0013 | ⚠️ Warning | Property-level mapping attributes are ignored under `[MapProfile]` | Use the `[Map...Member]` attributes instead |
| SBM0014 | ❌ Error | `[Map]` and `[MapProfile]` are both specified on the type | Specify either `[Map]` or `[MapProfile]`, not both |
| SBM0015 | ⚠️ Warning | Member size is not statically known, so overlap and size validation skip the member | Use a member type with a statically known size if validation is needed |

## Endpoint binding

| ID | Severity | Description | How to fix |
|---|---|---|---|
| SBM1001 | ❌ Error | `[ByteReader]` has no matching `[ByteWriter]` for the same entity and profile | Add the matching `[ByteWriter]` |
| SBM1002 | ❌ Error | Entity has no `[Map]` or `[MapProfile]` declaring a positive size | Declare a positive size with `[Map]` or `[MapProfile]` |
