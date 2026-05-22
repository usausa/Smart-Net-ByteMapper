namespace Smart.IO.ByteMapper.AspNetCore.Formatters;

using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;

// MVC OutputFormatter that serialises objects to binary using ByteMapper source-generated mappers.
// AOT/Trim safe: no reflection after startup.
public sealed class ByteMapperOutputFormatter : OutputFormatter
{
    private readonly ByteMapperRegistry registry;
    private readonly ByteMapperFormatterOptions options;

    public ByteMapperOutputFormatter(ByteMapperRegistry registry, ByteMapperFormatterOptions options)
    {
        this.registry = registry;
        this.options = options;

        foreach (var mediaType in options.SupportedMediaTypes)
        {
            SupportedMediaTypes.Add(mediaType);
        }
    }

    protected override bool CanWriteType(Type? type)
    {
        if (type is null) return false;

        if (type.IsArray)
        {
            var elem = type.GetElementType()!;
            return registry.GetBinding(elem) is not null;
        }

        // IEnumerable<T>
        var enumElem = GetEnumerableElementType(type);
        if (enumElem is not null)
        {
            return registry.GetBinding(enumElem) is not null;
        }

        return registry.GetBinding(type) is not null;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "Type is a well-known registered ByteMapper entity type; interface metadata is preserved by the source generator.")]
    private static Type? GetEnumerableElementType(Type type)
    {
        // Type itself may be IEnumerable<T>
        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType &&
                iface.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
            {
                return iface.GetGenericArguments()[0];
            }
        }

        return null;
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var objectType = context.ObjectType;
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var profile = httpContext.Items[ByteMapperConst.ProfileKey] as Type;

        if (objectType is not null && objectType.IsArray)
        {
            var elementType = objectType.GetElementType()!;
            var binding = (profile is not null ? registry.GetBinding(elementType, profile) : null)
                          ?? registry.GetBinding(elementType);
            if (binding is not null && context.Object is IEnumerable enumerable)
            {
                await WriteEnumerableAsync(binding, enumerable, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
            }

            return;
        }

        // IEnumerable<T>
        if (objectType is not null)
        {
            var enumElem = GetEnumerableElementType(objectType);
            if (enumElem is not null)
            {
                var binding = (profile is not null ? registry.GetBinding(enumElem, profile) : null)
                              ?? registry.GetBinding(enumElem);
                if (binding is not null && context.Object is IEnumerable enumerable)
                {
                    await WriteEnumerableAsync(binding, enumerable, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
                }

                return;
            }
        }

        if (objectType is not null)
        {
            var binding = (profile is not null ? registry.GetBinding(objectType, profile) : null)
                          ?? registry.GetBinding(objectType);
            if (binding is not null && context.Object is not null)
            {
                await WriteSingleAsync(binding, context.Object, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask WriteSingleAsync(
        ByteMapperBinding binding,
        object value,
        System.IO.Stream body,
        System.Threading.CancellationToken cancellationToken)
    {
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            binding.Write(value, buffer.AsSpan(0, size));
            await body.WriteAsync(buffer.AsMemory(0, size), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async ValueTask WriteEnumerableAsync(
        ByteMapperBinding elementBinding,
        IEnumerable enumerable,
        System.IO.Stream body,
        System.Threading.CancellationToken cancellationToken)
    {
        var elementSize = elementBinding.Size;
        var bufferSize = Math.Max(options.BufferSize, elementSize);
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            var pos = 0;
            var limit = buffer.Length - elementSize;

            foreach (var item in enumerable)
            {
                elementBinding.Write(item, buffer.AsSpan(pos, elementSize));
                pos += elementSize;

                if (pos > limit)
                {
                    await body.WriteAsync(buffer.AsMemory(0, pos), cancellationToken).ConfigureAwait(false);
                    pos = 0;
                }
            }

            if (pos > 0)
            {
                await body.WriteAsync(buffer.AsMemory(0, pos), cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
