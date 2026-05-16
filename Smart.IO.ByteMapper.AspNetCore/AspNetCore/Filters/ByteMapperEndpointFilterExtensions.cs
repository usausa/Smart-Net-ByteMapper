namespace Smart.AspNetCore.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Smart.AspNetCore.Formatters;

public static class ByteMapperEndpointFilterExtensions
{
    public static IEndpointConventionBuilder WithByteMapperFilter(this IEndpointConventionBuilder builder, ByteMapperFormatterConfig config, string profile = null)
    {
        return builder.AddEndpointFilter(new ByteMapperEndpointFilter(config, profile));
    }
}
