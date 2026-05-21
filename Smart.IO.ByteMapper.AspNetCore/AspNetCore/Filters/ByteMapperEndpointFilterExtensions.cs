namespace Smart.AspNetCore.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Smart.AspNetCore.Formatters;

public static class ByteMapperEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithByteMapperFilter(this RouteHandlerBuilder builder, ByteMapperFormatterConfig config, string profile = null)
    {
        return builder.AddEndpointFilter(new ByteMapperEndpointFilter(config, profile));
    }
}
