namespace Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

internal sealed record EndPointCollection(EquatableArray<EndPointModel> EndPoints)
{
    public static EndPointCollection Empty { get; } = new(EquatableArray<EndPointModel>.Empty);
}
