namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

internal sealed record EndpointCollection(EquatableArray<EndpointModel> Endpoints)
{
    public static EndpointCollection Empty { get; } = new(EquatableArray<EndpointModel>.Empty);
}
