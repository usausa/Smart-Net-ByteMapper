namespace Smart.IO.ByteMapper;

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.AspNetCore.Formatters;

// ---- テスト用エンティティ/プロファイル ----

internal sealed class FormatterEntity
{
    public byte Value { get; set; }
}

#pragma warning disable CA1812
internal sealed class FormatterProfile
{
}
#pragma warning restore CA1812

// MVC フォーマッターのプロファイル解決を検証する。
//   * プロファイル指定時は (entity, profile) を厳密に解決し、未登録はデフォルトへフォールバックせず例外
//   * プロファイル未指定時はデフォルトバインディングの存在が交渉 (CanRead/CanWriteResult) の条件
public class ByteMapperFormatterTests
{
    private const string MediaType = "application/octet-stream";

    // default: 8バイトレコード / profile: 4バイトレコード(どちらも先頭バイトが Value)
    private static ByteMapperRegistry CreateRegistry(bool withDefault, bool withProfile)
    {
        var single = new Dictionary<(Type, Type?), ByteMapperBinding>();
        if (withDefault)
        {
            single[(typeof(FormatterEntity), null)] = new ByteMapperBinding<FormatterEntity>(
                8,
                static (s, t) => t.Value = s[0],
                static (s, d) => d[0] = s.Value,
                static () => new FormatterEntity());
        }
        if (withProfile)
        {
            single[(typeof(FormatterEntity), typeof(FormatterProfile))] = new ByteMapperBinding<FormatterEntity>(
                4,
                static (s, t) => t.Value = s[0],
                static (s, d) => d[0] = s.Value,
                static () => new FormatterEntity());
        }
        return new ByteMapperRegistry(single, new Dictionary<(Type, Type?), object>());
    }

    private static ByteMapperInputFormatter CreateInputFormatter(bool withDefault, bool withProfile) =>
        new(CreateRegistry(withDefault, withProfile), new ByteMapperFormatterOptions());

    private static ByteMapperOutputFormatter CreateOutputFormatter(bool withDefault, bool withProfile) =>
        new(CreateRegistry(withDefault, withProfile), new ByteMapperFormatterOptions());

    private static DefaultHttpContext CreateHttpContext(Type? profile, byte[]? requestBody = null)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                ContentType = MediaType,
                Body = new MemoryStream(requestBody ?? [])
            },
            Response =
            {
                Body = new MemoryStream()
            }
        };
        if (profile is not null)
        {
            httpContext.Items[ByteMapperConst.ProfileKey] = profile;
        }
        return httpContext;
    }

    private static InputFormatterContext CreateInputContext(HttpContext httpContext, Type modelType) =>
        new(
            httpContext,
            "model",
            new ModelStateDictionary(),
            new EmptyModelMetadataProvider().GetMetadataForType(modelType),
            static (stream, encoding) => new StreamReader(stream, encoding));

    private static OutputFormatterWriteContext CreateOutputContext(HttpContext httpContext, Type objectType, object? value) =>
        new(
            httpContext,
            static (stream, encoding) => new StreamWriter(stream, encoding),
            objectType,
            value)
        {
            ContentType = new StringSegment(MediaType)
        };

    //--------------------------------------------------------------------------------
    // CanRead / CanWriteResult
    //--------------------------------------------------------------------------------

    [Fact]
    public void WhenDefaultBindingExistsThenCanReadIsTrue()
    {
        var formatter = CreateInputFormatter(withDefault: true, withProfile: false);
        var context = CreateInputContext(CreateHttpContext(profile: null), typeof(FormatterEntity));

        Assert.True(formatter.CanRead(context));
    }

    [Fact]
    public void WhenOnlyProfileBindingsAndNoProfileThenCanReadIsFalse()
    {
        var formatter = CreateInputFormatter(withDefault: false, withProfile: true);
        var context = CreateInputContext(CreateHttpContext(profile: null), typeof(FormatterEntity));

        Assert.False(formatter.CanRead(context));
    }

    [Fact]
    public void WhenOnlyProfileBindingsAndNoProfileThenCanReadIsFalseForArray()
    {
        var formatter = CreateInputFormatter(withDefault: false, withProfile: true);
        var context = CreateInputContext(CreateHttpContext(profile: null), typeof(FormatterEntity[]));

        Assert.False(formatter.CanRead(context));
    }

    [Fact]
    public void WhenProfileDeclaredThenCanReadIsTrue()
    {
        // プロファイル指定時は引き受け、未登録は ReadRequestBodyAsync 側で設定エラーにする
        var formatter = CreateInputFormatter(withDefault: true, withProfile: false);
        var context = CreateInputContext(CreateHttpContext(typeof(FormatterProfile)), typeof(FormatterEntity));

        Assert.True(formatter.CanRead(context));
    }

    [Fact]
    public void WhenOnlyProfileBindingsAndNoProfileThenCanWriteResultIsFalse()
    {
        var formatter = CreateOutputFormatter(withDefault: false, withProfile: true);
        var context = CreateOutputContext(CreateHttpContext(profile: null), typeof(FormatterEntity), new FormatterEntity());

        Assert.False(formatter.CanWriteResult(context));
    }

    [Fact]
    public void WhenDefaultBindingExistsThenCanWriteResultIsTrue()
    {
        var formatter = CreateOutputFormatter(withDefault: true, withProfile: false);
        var context = CreateOutputContext(CreateHttpContext(profile: null), typeof(FormatterEntity), new FormatterEntity());

        Assert.True(formatter.CanWriteResult(context));
    }

    //--------------------------------------------------------------------------------
    // 厳密なプロファイル解決(フォールバック除去)
    //--------------------------------------------------------------------------------

    [Fact]
    public async Task WhenDeclaredProfileBindingMissingThenReadThrows()
    {
        // 修正前はデフォルトレイアウトへ無言フォールバックして誤ったフレーミングで読んでいた
        var formatter = CreateInputFormatter(withDefault: true, withProfile: false);
        var context = CreateInputContext(CreateHttpContext(typeof(FormatterProfile), new byte[8]), typeof(FormatterEntity));

        await Assert.ThrowsAsync<InvalidOperationException>(() => formatter.ReadRequestBodyAsync(context));
    }

    [Fact]
    public async Task WhenDeclaredProfileBindingMissingThenWriteThrows()
    {
        var formatter = CreateOutputFormatter(withDefault: true, withProfile: false);
        var context = CreateOutputContext(CreateHttpContext(typeof(FormatterProfile)), typeof(FormatterEntity), new FormatterEntity());

        await Assert.ThrowsAsync<InvalidOperationException>(() => formatter.WriteResponseBodyAsync(context));
    }

    //--------------------------------------------------------------------------------
    // プロファイル/デフォルトの選択
    //--------------------------------------------------------------------------------

    [Fact]
    public async Task WhenProfileBindingExistsThenReadUsesProfileSize()
    {
        var formatter = CreateInputFormatter(withDefault: true, withProfile: true);
#pragma warning disable IDE0230
        var body = new byte[] { 0x7B, 0x00, 0x00, 0x00 };  // プロファイルサイズ = 4
#pragma warning restore IDE0230
        var context = CreateInputContext(CreateHttpContext(typeof(FormatterProfile), body), typeof(FormatterEntity));

        var result = await formatter.ReadRequestBodyAsync(context);

        var model = Assert.IsType<FormatterEntity>(result.Model);
        Assert.Equal(0x7B, model.Value);
    }

    [Fact]
    public async Task WhenProfileBindingExistsThenWriteUsesProfileSize()
    {
        var formatter = CreateOutputFormatter(withDefault: true, withProfile: true);
        var httpContext = CreateHttpContext(typeof(FormatterProfile));
        var context = CreateOutputContext(httpContext, typeof(FormatterEntity), new FormatterEntity { Value = 0x7B });

        await formatter.WriteResponseBodyAsync(context);

        var written = ((MemoryStream)httpContext.Response.Body).ToArray();
        Assert.Equal(4, written.Length);
        Assert.Equal(0x7B, written[0]);
    }

    [Fact]
    public async Task WhenNoProfileThenWriteUsesDefaultSize()
    {
        var formatter = CreateOutputFormatter(withDefault: true, withProfile: true);
        var httpContext = CreateHttpContext(profile: null);
        var context = CreateOutputContext(httpContext, typeof(FormatterEntity), new FormatterEntity { Value = 0x01 });

        await formatter.WriteResponseBodyAsync(context);

        var written = ((MemoryStream)httpContext.Response.Body).ToArray();
        Assert.Equal(8, written.Length);
        Assert.Equal(0x01, written[0]);
    }
}
