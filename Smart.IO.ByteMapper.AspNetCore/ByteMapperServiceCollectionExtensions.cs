namespace Smart.IO.ByteMapper.AspNetCore;

using System;

using Microsoft.Extensions.DependencyInjection;

public static class ByteMapperServiceCollectionExtensions
{
    public static IServiceCollection AddByteMapperFormatters(
        this IServiceCollection services,
        ByteMapperRegistry registry,
        Action<ByteMapperFormatterOptions>? configure = null)
    {
        var options = new ByteMapperFormatterOptions();
        configure?.Invoke(options);

        services.AddSingleton(registry);
        services.AddSingleton(options);
        services.AddSingleton<Formatters.ByteMapperInputFormatter>();
        services.AddSingleton<Formatters.ByteMapperOutputFormatter>();

        return services;
    }
}
