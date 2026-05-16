namespace Smart.AspNetCore.Filters;

using System.Buffers;

using Microsoft.AspNetCore.Http;

using Smart.AspNetCore.Formatters;
using Smart.IO.ByteMapper;
using Smart.Reflection;

/// <summary>
/// An <see cref="IEndpointFilter"/> that deserializes the request body from fixed-length binary
/// (using ByteMapper) into a parameter of the endpoint handler, and serializes the response
/// object back to fixed-length binary.
/// </summary>
/// <remarks>
/// Usage with Minimal API:
/// <code>
/// app.MapPost("/data", (SampleData data) => data)
///    .AddEndpointFilter(new ByteMapperEndpointFilter(config));
/// </code>
/// The filter attempts to deserialize each handler parameter (in order) whose type has a
/// registered ByteMapper mapper. The return value is serialized back with Content-Type
/// <c>application/octet-stream</c>.
/// </remarks>
public sealed class ByteMapperEndpointFilter : IEndpointFilter
{
    private readonly MapperFactory mapperFactory;

    private readonly IDelegateFactory delegateFactory;

    private readonly string profile;

    public ByteMapperEndpointFilter(ByteMapperFormatterConfig config, string profile = null)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        mapperFactory = config.MapperFactory;
        delegateFactory = config.DelegateFactory;
        this.profile = profile;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        await DeserializeRequestAsync(context).ConfigureAwait(false);

        var result = await next(context).ConfigureAwait(false);
        if (result is not null)
        {
            await SerializeResponseAsync(context.HttpContext, result).ConfigureAwait(false);
        }

        return null;
    }

    private async ValueTask DeserializeRequestAsync(EndpointFilterInvocationContext context)
    {
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;

        for (var i = 0; i < context.Arguments.Count; i++)
        {
            var argType = context.Arguments[i]?.GetType();
            if (argType is null)
            {
                continue;
            }

            if (argType == typeof(HttpContext) || argType == typeof(CancellationToken))
            {
                continue;
            }

            var mapper = TryGetMapper(argType);
            if (mapper is null)
            {
                continue;
            }

            var factory = ObjectFactoryCache.GetOrCreate(argType, delegateFactory);
            var bufferLength = mapper.Size;
            var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            try
            {
#pragma warning disable CA1835
                var read = await httpContext.Request.Body.ReadAsync(buffer.AsMemory(0, bufferLength), cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1835
                if (read == bufferLength)
                {
                    var target = factory();
                    mapper.FromByte(buffer.AsSpan(0, bufferLength), target);
                    context.Arguments[i] = target;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            break;
        }
    }

    private async ValueTask SerializeResponseAsync(HttpContext httpContext, object result)
    {
        var resultType = result.GetType();
        var mapper = TryGetMapper(resultType);
        if (mapper is null)
        {
            return;
        }

        var bufferLength = mapper.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
        try
        {
            mapper.ToByte(buffer.AsSpan(0, bufferLength), result);

            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.ContentLength = bufferLength;
#pragma warning disable CA1835
            await httpContext.Response.Body.WriteAsync(buffer.AsMemory(0, bufferLength), httpContext.RequestAborted).ConfigureAwait(false);
#pragma warning restore CA1835
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private ITypeMapper TryGetMapper(Type type)
    {
#pragma warning disable CA1031
        try
        {
            return String.IsNullOrEmpty(profile)
                ? mapperFactory.Create(type)
                : mapperFactory.Create(type, profile);
        }
        catch
        {
            return null;
        }
#pragma warning restore CA1031
    }

    private static class ObjectFactoryCache
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object>> Cache = new();

        internal static Func<object> GetOrCreate(Type type, IDelegateFactory delegateFactory)
        {
            return Cache.GetOrAdd(type, delegateFactory.CreateFactory);
        }
    }
}
