namespace Smart.IO.ByteMapper.AspNetCore;

using System;

using Microsoft.Extensions.DependencyInjection;

// Extension methods for registering ByteMapper services.
// The overload that takes a ByteMapperRegistry is intended for direct use;
// the parameterless overload is emitted by the source generator into the user's assembly.
public static class ByteMapperServiceCollectionExtensions
{
    // Registers ByteMapperRegistry, ByteMapperInputFormatter, and ByteMapperOutputFormatter as singleton services.
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
