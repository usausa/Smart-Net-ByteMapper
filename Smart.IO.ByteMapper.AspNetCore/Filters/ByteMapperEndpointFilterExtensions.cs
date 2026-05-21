namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;
using System.Buffers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Strongly-typed extension methods for registering ByteMapper body binding on
/// Minimal API route handlers.  No reflection is used: <c>typeof(T)</c> resolves
/// the binding at compile time.
/// </summary>
public static class ByteMapperEndpointFilterExtensions
{
    // ----------------------------------------------------------------
    // Single entity
    // ----------------------------------------------------------------

    /// <summary>
    /// Reads the HTTP request body as <typeparamref name="T"/> using the
    /// registered <see cref="ByteMapperBinding{T}"/>, and writes the return
    /// value back as binary when the handler returns a <typeparamref name="T"/>.
    /// </summary>
    public static RouteHandlerBuilder WithByteMapperBody<T>(
        this RouteHandlerBuilder builder,
        string? profile = null)
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetBinding<T>(profile)
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperBinding registered for {typeof(T).FullName} (profile={profile ?? "default"}).");

                await ReadIntoArgumentAsync(invocationContext, binding).ConfigureAwait(false);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is T typed)
                {
                    return new ByteMapperResult<T>(typed, binding);
                }

                return result;
            });

    // ----------------------------------------------------------------
    // Array
    // ----------------------------------------------------------------

    /// <summary>
    /// Reads the HTTP request body as <typeparamref name="T"/>[] using the
    /// registered <see cref="ByteMapperArrayBinding{T}"/>.
    /// </summary>
    public static RouteHandlerBuilder WithByteMapperArrayBody<T>(
        this RouteHandlerBuilder builder,
        string? profile = null)
        => builder.AddEndpointFilterFactory((_, next) =>
            async invocationContext =>
            {
                var registry = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetArrayBinding<T>(profile)
                    ?? throw new InvalidOperationException(
                        $"No ByteMapperArrayBinding registered for {typeof(T).FullName} (profile={profile ?? "default"}).");

                var items = await ReadArrayAsync(invocationContext.HttpContext, binding).ConfigureAwait(false);
                ReplaceArgument(invocationContext, items);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is T[] typedArray)
                {
                    return new ByteMapperArrayResult<T>(typedArray, binding);
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

            if (read < size) return;

            var target = binding.Create();
            binding.Read(buffer.AsSpan(0, size), target);
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
        var items = new System.Collections.Generic.List<T>();
        try
        {
            int read;
            while ((read = await httpContext.Request.Body
                       .ReadAsync(buffer.AsMemory(0, elementSize), httpContext.RequestAborted)
                       .ConfigureAwait(false)) == elementSize)
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

        return items.ToArray();
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
