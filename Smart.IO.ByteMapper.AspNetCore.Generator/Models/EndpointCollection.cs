namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// All bindings parsed from one [ByteMapperEndpoint] class.
// Result<T> requires a single equatable value, so the per-class bindings are wrapped here and
// unpacked with SelectMany at the pipeline stage.
internal sealed record EndpointCollection(EquatableArray<EndpointModel> Endpoints)
{
    public static EndpointCollection Empty { get; } = new(EquatableArray<EndpointModel>.Empty);
}
