namespace Smart.IO.ByteMapper.AspNetCore.Formatters;

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;

/// <summary>
/// MVC <see cref="InputFormatter"/> that deserialises binary bodies using
/// ByteMapper source-generated mappers.  AOT/Trim safe: no reflection after
/// startup.
/// </summary>
public sealed class ByteMapperInputFormatter : InputFormatter
{
    private readonly ByteMapperRegistry registry;
    private readonly ByteMapperFormatterOptions options;

    public ByteMapperInputFormatter(ByteMapperRegistry registry, ByteMapperFormatterOptions options)
    {
        this.registry = registry;
        this.options = options;

        foreach (var mediaType in options.SupportedMediaTypes)
        {
            SupportedMediaTypes.Add(mediaType);
        }
    }

    protected override bool CanReadType(Type type)
    {
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

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var modelType = context.ModelType;
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var profile = httpContext.Items[ByteMapperConst.ProfileKey] as string;

        if (modelType.IsArray)
        {
            var elementType = modelType.GetElementType()!;
            var binding = (profile is not null ? registry.GetBinding(elementType, profile) : null)
                          ?? registry.GetBinding(elementType);
            if (binding is null)
            {
                return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
            }

            var result = await ReadArrayAsync(binding, elementType, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }

        // IEnumerable<T> — read as array, return as array (assignable to IEnumerable<T>)
        // Suppressed: modelType is a well-known registered type; GetInterfaces is safe here.
        var enumElemType = GetEnumerableElementType(modelType);
        if (enumElemType is not null)
        {
            var binding = (profile is not null ? registry.GetBinding(enumElemType, profile) : null)
                          ?? registry.GetBinding(enumElemType);
            if (binding is null)
            {
                return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
            }

            var result = await ReadArrayAsync(binding, enumElemType, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }

        {
            var binding = (profile is not null ? registry.GetBinding(modelType, profile) : null)
                          ?? registry.GetBinding(modelType);
            if (binding is null)
            {
                return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
            }

            var result = await ReadSingleAsync(binding, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }
    }

    private static async ValueTask<object?> ReadSingleAsync(
        ByteMapperBinding binding,
        System.IO.Stream body,
        System.Threading.CancellationToken cancellationToken)
    {
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var read = await body.ReadAsync(buffer.AsMemory(0, size), cancellationToken).ConfigureAwait(false);
            if (read < size)
            {
                return null;
            }

            var target = binding.Factory();
            binding.Read(buffer.AsSpan(0, size), target);
            return target;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async ValueTask<Array> ReadArrayAsync(
        ByteMapperBinding elementBinding,
        Type elementType,
        System.IO.Stream body,
        System.Threading.CancellationToken cancellationToken)
    {
        var elementSize = elementBinding.Size;
        var bufferSize = Math.Max(options.BufferSize, elementSize);
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        var items = new System.Collections.Generic.List<object>();
        try
        {
            int read;
            while ((read = await body.ReadAsync(buffer.AsMemory(0, elementSize), cancellationToken).ConfigureAwait(false)) == elementSize)
            {
                var target = elementBinding.Factory();
                elementBinding.Read(buffer.AsSpan(0, elementSize), target);
                items.Add(target);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        var array = CreateArrayInstance(elementType, items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            array.SetValue(items[i], i);
        }

        return array;
    }

    [UnconditionalSuppressMessage("AotAnalysis", "IL3050", Justification = "elementType is a registered ByteMapper entity type; array creation is safe at runtime.")]
    private static Array CreateArrayInstance(Type elementType, int count) => Array.CreateInstance(elementType, count);
}
