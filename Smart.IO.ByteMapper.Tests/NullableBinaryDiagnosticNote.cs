namespace Smart.IO.ByteMapper;

// This file contains compile-time verification source for SBM0008.
// The source below intentionally does NOT compile when uncommented,
// because [MapBinary<int>] must not be applied to an int? (Nullable<int>) property.
// The source generator emits SBM0008 (error) in that case.
//
// To manually verify: uncomment the block below, build, and confirm:
//   error SBM0008: Unsupported type for MapBinary. method=[Read], property=[Value]
//
// [Map(4, UseDelimiter = false)]
// internal sealed class NullableBinaryRecord
// {
//     [MapBinary<int>(0)]         // SBM0008 — int? is not allowed
//     public int? Value { get; set; }
// }
//
// internal static partial class NullableBinaryMapper
// {
//     [ByteReader]
//     public static partial void Read(ReadOnlySpan<byte> buffer, NullableBinaryRecord target);
// }
