namespace Smart.IO.ByteMapper.AspNetCore;

using System;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering ByteMapper services.
/// The overload that takes a <see cref="ByteMapperRegistry"/> is intended for
/// direct use; the parameterless overload is emitted by the source generator
/// into the user's assembly and calls this method internally.
/// </summary>
public static class ByteMapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ByteMapperRegistry"/>, <see cref="Formatters.ByteMapperInputFormatter"/>,
    /// and <see cref="Formatters.ByteMapperOutputFormatter"/> as singleton services.
    /// </summary>
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
