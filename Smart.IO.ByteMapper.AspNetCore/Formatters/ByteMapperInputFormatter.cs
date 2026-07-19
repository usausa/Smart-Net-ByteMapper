namespace Smart.IO.ByteMapper.AspNetCore.Formatters;

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;

// MVC InputFormatter that deserialises binary bodies using ByteMapper source-generated mappers.
// AOT/Trim safe: no reflection after startup.
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
            return registry.HasAnyBinding(type.GetElementType()!);
        }

        // IEnumerable<T>
        var enumElem = GetEnumerableElementType(type);
        if (enumElem is not null)
        {
            return registry.HasAnyBinding(enumElem);
        }

        return registry.HasAnyBinding(type);
    }

    // Per-request refinement over CanReadType: when no profile is declared the default binding must
    // exist (an entity registered only under profiles is not readable without one). With a profile
    // declared the request is claimed, and a missing profile binding surfaces as a configuration
    // error in ReadRequestBodyAsync instead of being hidden by content negotiation.
    // CanReadType のリクエスト単位の絞り込み: プロファイル未指定時はデフォルトバインディングの存在を
    // 要求する（プロファイルのみ登録のエンティティはプロファイル無しでは読めない）。プロファイル指定時は
    // リクエストを引き受け、バインディング未登録は交渉で隠さず ReadRequestBodyAsync で設定エラーとして表面化させる。
    public override bool CanRead(InputFormatterContext context)
    {
        if (!base.CanRead(context))
        {
            return false;
        }

        if (context.HttpContext.Items[ByteMapperConst.ProfileKey] is Type)
        {
            return true;
        }

        return registry.GetBinding(ResolveElementType(context.ModelType)) is not null;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2070", Justification = "Type is a well-known registered ByteMapper entity type; interface metadata is preserved by the source generator.")]
    private static Type? GetEnumerableElementType(Type type)
    {
        // Type itself may be IEnumerable<T>
        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
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
        var profile = httpContext.Items[ByteMapperConst.ProfileKey] as Type;

        if (modelType.IsArray)
        {
            var elementType = modelType.GetElementType()!;
            var binding = GetRequiredBinding(elementType, profile);
            var result = await ReadArrayAsync(binding, elementType, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }

        // IEnumerable<T> — read as array, return as array (assignable to IEnumerable<T>)
        // Suppressed: modelType is a well-known registered type; GetInterfaces is safe here.
        var enumElemType = GetEnumerableElementType(modelType);
        if (enumElemType is not null)
        {
            var binding = GetRequiredBinding(enumElemType, profile);
            var result = await ReadArrayAsync(binding, enumElemType, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }

        {
            var binding = GetRequiredBinding(modelType, profile);
            var result = await ReadSingleAsync(binding, httpContext.Request.Body, cancellationToken).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }
    }

    // Resolves the binding for the current request. A declared profile must resolve exactly — falling
    // back to the default layout would silently mis-frame the data — and a missing binding is a server
    // configuration error reported like the Minimal API filters do.
    // 現在のリクエストのバインディングを解決する。プロファイル指定時は厳密に解決する（デフォルトレイアウトへの
    // フォールバックはデータのフレーミングを黙って壊す）。未登録は Minimal API フィルターと同様にサーバー設定エラー。
    private ByteMapperBinding GetRequiredBinding(Type elementType, Type? profile)
    {
        var binding = profile is not null
            ? registry.GetBinding(elementType, profile)
            : registry.GetBinding(elementType);
        return binding ?? throw new InvalidOperationException(
            $"No ByteMapperBinding registered for {elementType.FullName} (profile={(profile is null ? "default" : profile.FullName)}).");
    }

    private static Type ResolveElementType(Type type)
        => type.IsArray ? type.GetElementType()! : GetEnumerableElementType(type) ?? type;

    private static async ValueTask<object?> ReadSingleAsync(
        ByteMapperBinding binding,
        Stream body,
        CancellationToken cancellationToken)
    {
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            if (!await body.ReadExactAsync(buffer, size, cancellationToken).ConfigureAwait(false))
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
        Stream body,
        CancellationToken cancellationToken)
    {
        var items = new List<object>();
        await body.ReadRecordsAsync(
            elementBinding.Size,
            options.BufferSize,
            (items, binding: elementBinding),
            static (s, mem) =>
            {
                var target = s.binding.Factory();
                s.binding.Read(mem.Span, target);
                s.items.Add(target);
            },
            cancellationToken).ConfigureAwait(false);

        var array = CreateArrayInstance(elementType, items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            array.SetValue(items[i], i);
        }

        return array;
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "elementType is a registered ByteMapper entity type; array creation is safe at runtime.")]
    private static Array CreateArrayInstance(Type elementType, int count) => Array.CreateInstance(elementType, count);
}
