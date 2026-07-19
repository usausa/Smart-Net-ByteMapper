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
        if (type is null)
        {
            return false;
        }

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

    // Per-request refinement over CanWriteType: when no profile is declared the default binding must
    // exist, so an entity registered only under profiles no longer negotiates successfully and then
    // produces an empty response. With a profile declared the request is claimed, and a missing
    // profile binding surfaces as a configuration error in WriteResponseBodyAsync.
    // CanWriteType のリクエスト単位の絞り込み: プロファイル未指定時はデフォルトバインディングの存在を
    // 要求し、プロファイルのみ登録のエンティティが交渉を通過して空レスポンスになる事態を防ぐ。
    // プロファイル指定時はリクエストを引き受け、未登録は WriteResponseBodyAsync で設定エラーとして表面化させる。
    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        if (!base.CanWriteResult(context))
        {
            return false;
        }

        var objectType = context.ObjectType;
        if (objectType is null)
        {
            return false;
        }

        if (context.HttpContext.Items[ByteMapperConst.ProfileKey] is Type)
        {
            return true;
        }

        return registry.GetBinding(ResolveElementType(objectType)) is not null;
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

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var objectType = context.ObjectType;
        if (objectType is null)
        {
            return;
        }

        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var profile = httpContext.Items[ByteMapperConst.ProfileKey] as Type;

        if (objectType.IsArray)
        {
            var binding = GetRequiredBinding(objectType.GetElementType()!, profile);
            if (context.Object is IEnumerable enumerable)
            {
                await WriteEnumerableAsync(binding, enumerable, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
            }

            return;
        }

        // IEnumerable<T>
        var enumElem = GetEnumerableElementType(objectType);
        if (enumElem is not null)
        {
            var binding = GetRequiredBinding(enumElem, profile);
            if (context.Object is IEnumerable enumerable)
            {
                await WriteEnumerableAsync(binding, enumerable, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
            }

            return;
        }

        {
            var binding = GetRequiredBinding(objectType, profile);
            if (context.Object is not null)
            {
                await WriteSingleAsync(binding, context.Object, httpContext.Response.Body, cancellationToken).ConfigureAwait(false);
            }
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

    private static async ValueTask WriteSingleAsync(
        ByteMapperBinding binding,
        object value,
        Stream body,
        CancellationToken cancellationToken)
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
        Stream body,
        CancellationToken cancellationToken)
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
