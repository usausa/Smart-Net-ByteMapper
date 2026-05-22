namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;
using System.Buffers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// Strongly-typed extension methods for registering ByteMapper body binding on Minimal API route handlers.
// No reflection is used: typeof(T) resolves the binding at compile time.
// For POST/PUT endpoints the parsed value is stored in HttpContext.Items under the key typeof(T)
// so that handlers can retrieve it without triggering ASP.NET Core's JSON body binder.
// Use GetByteMapperBody<T> to retrieve the value inside a handler.
public static class ByteMapperEndpointFilterExtensions
{
    // Retrieves a body value parsed by WithByteMapperBody<T> from HttpContext.Items.
    // Returns null if the filter has not run or the body was too short.
    public static T? GetByteMapperBody<T>(this HttpContext httpContext)
        where T : class
        => httpContext.Items[typeof(T)] as T;

    // Retrieves an array body value parsed by WithByteMapperArrayBody<T> from HttpContext.Items.
    // Returns null if the filter has not run.
    public static T[]? GetByteMapperArrayBody<T>(this HttpContext httpContext)
        => httpContext.Items[typeof(T[])] as T[];
    // ----------------------------------------------------------------
    // Single entity
    // ----------------------------------------------------------------

    // Reads the HTTP request body as T using the registered ByteMapperBinding<T>,
    // and writes the return value back as binary when the handler returns a T.
    public static RouteHandlerBuilder WithByteMapperBody<T>(
        this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetBinding<T>()
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperBinding registered for {typeof(T).FullName} (profile=default).");

                await ReadIntoArgumentAsync(invocationContext, binding).ConfigureAwait(false);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is T typed)
                {
                    return new ByteMapperResult<T>(typed, binding);
                }

                return result;
            });

    // Reads the HTTP request body as TEntity using the registered ByteMapperBinding<T> for the specified
    // profile, and writes the return value back as binary when the handler returns a TEntity.
    public static RouteHandlerBuilder WithByteMapperBody<TEntity, TProfile>(
        this RouteHandlerBuilder builder)
        where TProfile : class
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetBinding<TEntity, TProfile>()
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperBinding registered for {typeof(TEntity).FullName} (profile={typeof(TProfile).FullName}).");

                await ReadIntoArgumentAsync(invocationContext, binding).ConfigureAwait(false);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is TEntity typed)
                {
                    return new ByteMapperResult<TEntity>(typed, binding);
                }

                return result;
            });

    // ----------------------------------------------------------------
    // Array
    // ----------------------------------------------------------------

    // Reads the HTTP request body as T[] using the registered ByteMapperArrayBinding<T>.
    public static RouteHandlerBuilder WithByteMapperArrayBody<T>(
        this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetArrayBinding<T>()
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperArrayBinding registered for {typeof(T).FullName} (profile=default).");

                var items = await ReadArrayAsync(invocationContext.HttpContext, binding).ConfigureAwait(false);
                ReplaceArgument(invocationContext, items);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is T[] typedArray)
                {
                    return new ByteMapperArrayResult<T>(typedArray, binding);
                }

                return result;
            });

    // Reads the HTTP request body as TEntity[] using the registered ByteMapperArrayBinding<T> for the specified profile.
    public static RouteHandlerBuilder WithByteMapperArrayBody<TEntity, TProfile>(
        this RouteHandlerBuilder builder)
        where TProfile : class
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetArrayBinding<TEntity, TProfile>()
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperArrayBinding registered for {typeof(TEntity).FullName} (profile={typeof(TProfile).FullName}).");

                var items = await ReadArrayAsync(invocationContext.HttpContext, binding).ConfigureAwait(false);
                ReplaceArgument(invocationContext, items);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is TEntity[] typedArray)
                {
                    return new ByteMapperArrayResult<TEntity>(typedArray, binding);
                }

                return result;
            });

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private static async ValueTask ReadIntoArgumentAsync<T>(
        EndpointFilterInvocationContext context,
        ByteMapperBinding<T> binding)
    {
        var httpContext = context.HttpContext;
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var read = await httpContext.Request.Body
                .ReadAsync(buffer.AsMemory(0, size), httpContext.RequestAborted)
                .ConfigureAwait(false);

            if (read < size)
            {
                return;
            }

            var target = binding.Create();
            binding.Read(buffer.AsSpan(0, size), target);

            // Store in HttpContext.Items so handlers that take HttpContext can retrieve it.
            httpContext.Items[typeof(T)] = target;
            // Also replace the matching argument slot if one exists (e.g. nullable T? param).
            ReplaceArgument(context, target);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static async ValueTask<T[]> ReadArrayAsync<T>(
        HttpContext httpContext,
        ByteMapperArrayBinding<T> binding)
    {
        var elementSize = binding.ElementSize;
        var buffer = ArrayPool<byte>.Shared.Rent(elementSize);
        var items = new List<T>();
        try
        {
            while ((await httpContext.Request.Body.ReadAsync(buffer.AsMemory(0, elementSize), httpContext.RequestAborted).ConfigureAwait(false)) == elementSize)
            {
                var item = binding.Factory();
                binding.ReadElement(buffer.AsSpan(0, elementSize), item);
                items.Add(item);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        var result = items.ToArray();
        // Store in HttpContext.Items so handlers that take HttpContext can retrieve it.
        httpContext.Items[typeof(T[])] = result;
        return result;
    }

    private static void ReplaceArgument<T>(EndpointFilterInvocationContext context, T value)
    {
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            if (context.Arguments[i] is T)
            {
                context.Arguments[i] = value;
                return;
            }
        }

        // If no slot was pre-populated (e.g. nullable), try null slot
        for (var i = 0; i < context.Arguments.Count; i++)
        {
            if (context.Arguments[i] is null)
            {
                context.Arguments[i] = value;
                return;
            }
        }
    }
}

// ----------------------------------------------------------------
// IResult implementations
// ----------------------------------------------------------------

internal sealed class ByteMapperResult<T> : IResult
{
    private readonly T value;
    private readonly ByteMapperBinding<T> binding;

    internal ByteMapperResult(T value, ByteMapperBinding<T> binding)
    {
        this.value = value;
        this.binding = binding;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            binding.Write(value, buffer.AsSpan(0, size));
            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.ContentLength = size;
            await httpContext.Response.Body
                .WriteAsync(buffer.AsMemory(0, size), httpContext.RequestAborted)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}

internal sealed class ByteMapperArrayResult<T> : IResult
{
    private readonly T[] values;
    private readonly ByteMapperArrayBinding<T> binding;

    internal ByteMapperArrayResult(T[] values, ByteMapperArrayBinding<T> binding)
    {
        this.values = values;
        this.binding = binding;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var elementSize = binding.ElementSize;
        var totalSize = elementSize * values.Length;
        var buffer = ArrayPool<byte>.Shared.Rent(elementSize);
        try
        {
            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.ContentLength = totalSize;
            foreach (var item in values)
            {
                binding.WriteElement(item, buffer.AsSpan(0, elementSize));
                await httpContext.Response.Body
                    .WriteAsync(buffer.AsMemory(0, elementSize), httpContext.RequestAborted)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
