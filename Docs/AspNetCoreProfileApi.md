# プロファイル指定 API 改善 実装指示書

最終更新: 2026-05-22

本書は、`Smart.IO.ByteMapper.AspNetCore` のプロファイル指定 API について、`Example.Web` で `ByteMapperRegistry` を直接参照している現状を解消し、**属性 (MVC) / 型付き拡張メソッド (Minimal API)** でプロファイルクラスを指定できるようにするための実装指示書である。

設計確定事項:
- プロファイル識別子は **プロファイルクラスの `Type`** に統一する。`[ByteMapperEndpoint]` の `string? Key` プロパティは **廃止** する。
- 属性の主形式は **`[ByteMapperProfile(typeof(TProfile))]`**(`typeof` 引数) とする。`[ByteMapperProfile<TProfile>]`(ジェネリック属性) は補助形式として提供する。
- ドキュメントは新規 `Docs/AspNetCoreProfileApi.md` として独立させる(本書)。

---

## 1. 目的・背景

### 1.1 目的

- `Example.Web` のアクション / Minimal API ハンドラ本体から `ByteMapperRegistry` への直接参照を **完全に排除** する。
- プロファイル指定を **コンパイル時に型チェック可能** にする(文字列キーの撲滅)。
- 既存の `ByteMapperFormatter`(MVC) / `EndpointFilter`(Minimal API) の枠組みを最大限活かす。

### 1.2 現状の問題点(Example.Web)

#### MVC コントローラ (`Example.Web/Controllers/MapController.cs`)

```csharp
public sealed class MapController(ByteMapperRegistry registry) : Controller   // 注入が必須
{
    [HttpGet]
    public IActionResult GetProfileCodeName()
    {
        var binding = registry.GetArrayBinding<SampleData>("code-name");        // 文字列キーで直引き
        if (binding is null)
        {
            return StatusCode(500, "Binding 'code-name' not registered.");
        }

        var data = CreateDummyData();
        var buffer = new byte[binding.ElementSize * data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            binding.WriteElement(data[i], buffer.AsSpan(i * binding.ElementSize, binding.ElementSize));
        }

        return File(buffer, "application/octet-stream");                        // 手書きシリアライズ
    }
}
```

問題:
1. `ByteMapperRegistry` をコントローラに注入し、`registry.GetArrayBinding<T>("code-name")` を直書きしている。
2. 文字列 `"code-name"` がハードコード。タイポすればランタイムまで気付けない。
3. バッファ確保・要素ループ・`File(...)` 返却まで全部手書き。Formatter の存在が無意味になっている。
4. 非プロファイル経路(`GetArray()` 等)は Formatter 経由で `SampleData[]` を返すだけだが、プロファイル経路だけ別アーキテクチャになっており **一貫性がない**。

#### Minimal API (`Example.Web/MinimalApi/SampleEndpoints.cs`)

```csharp
public static void MapSampleEndpoints(this WebApplication app, ByteMapperRegistry registry)  // 引数で受け取り
{
    group.MapGet("/profile/code-name", () =>
    {
        var binding = registry.GetArrayBinding<SampleData>("code-name")        // 直引き
            ?? throw new InvalidOperationException("Binding 'code-name' not registered.");
        var data = CreateDummyData();
        var buffer = new byte[binding.ElementSize * data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            binding.WriteElement(data[i], buffer.AsSpan(i * binding.ElementSize, binding.ElementSize));
        }
        return Results.Bytes(buffer, "application/octet-stream");
    });

    group.MapPost("/profile/code-name", async (HttpContext ctx) =>
    {
        var binding = registry.GetBinding<SampleData>("code-name")              // 直引き
            ?? throw new InvalidOperationException("Binding 'code-name' not registered.");
        var items = await ReadArrayAsync(ctx.Request.Body, binding, ctx.RequestAborted);
        return Results.Ok(new { count = items.Length });
    });
}
```

問題:
1. `MapSampleEndpoints` が `ByteMapperRegistry` を引数で取る。`Program.cs` も Registry を `var registry = __ByteMapperAspNetCoreBootstrap.Build();` で取り出して引き渡している。
2. プロファイル経路だけ `HttpContext` を直接扱う非対称な実装になっている。
3. 非プロファイル経路は `WithByteMapperBody<SampleData>()` / `WithByteMapperArrayBody<SampleData>()` を使っており、ここでも一貫性が崩れている。

### 1.3 ゴール

修正後の Example.Web は以下のように書けるべき:

```csharp
// MapController
[Produces("text/x-fixedrecord")]
[HttpGet]
[ByteMapperProfile(typeof(SampleDataCodeNameProfile))]                          // ← 属性 1 行
public SampleData[] GetProfileCodeName() => CreateDummyData();

[HttpPost]
[ByteMapperProfile(typeof(SampleDataCodeNameProfile))]
public IActionResult PostProfileCodeName([FromBody] SampleData[] values)        // 配列モデルバインド
    => Ok(new { count = values.Length });

// Minimal API
group.MapGet("/profile/code-name", () => CreateDummyData())
    .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>()           // ← 型パラメータで指定
    .WithName("MinimalGetProfileCodeName");

group.MapPost("/profile/code-name", (SampleData[] values)
        => Results.Ok(new { count = values.Length }))
    .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>();
```

`MapController` のコンストラクタ引数 `ByteMapperRegistry registry` は削除可能、`MapSampleEndpoints` の引数 `ByteMapperRegistry registry` も削除可能になる。

---

## 2. 設計方針

### 2.1 識別子の統一(必須)

`ByteMapperRegistry` のキーを `(Type Entity, string? Profile)` から **`(Type Entity, Type? Profile)`** に変更する。

| 変更前 | 変更後 |
| --- | --- |
| `(typeof(SampleData), "code-name")` | `(typeof(SampleData), typeof(SampleDataCodeNameProfile))` |
| `(typeof(SampleData), null)` | `(typeof(SampleData), null)` |

理由:
- プロファイルクラスは Generator がコード生成時に既に `Type` として扱っている (`[ByteReader(Profile = typeof(...))]`)。新たな間接層を持ち込まずに済む。
- 文字列キーは:
  - タイポ耐性なし
  - リファクタ追従なし
  - IntelliSense なし
  - すでに `Profile = typeof(...)` で書いている内容との **二重記述** になる
- `Type` は AOT 適合(`typeof(T)` はコンパイル時定数として埋め込まれる)。

### 2.2 `[ByteMapperEndpoint]` 属性の単純化(必須)

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ByteMapperEndpointAttribute : Attribute
{
    // public string? Key { get; init; }                ← 廃止
    public bool GenerateArrayBinding { get; init; } = true;
}
```

`Key` プロパティを **完全に削除** する。プロファイル識別子は `[ByteReader]` / `[ByteWriter]` の `Profile = typeof(...)` から Generator が自動取得する。

### 2.3 MVC: 型ベースの `[ByteMapperProfile]` 属性(必須)

既存の `ByteMapperProfileAttribute(string profile)` を **置き換える**:

```csharp
namespace Smart.IO.ByteMapper.AspNetCore.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ByteMapperProfileAttribute : Attribute, IResourceFilter
{
    public Type ProfileType { get; }

    public ByteMapperProfileAttribute(Type profileType)
    {
        ProfileType = profileType ?? throw new ArgumentNullException(nameof(profileType));
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.HttpContext.Items[ByteMapperConst.ProfileKey] = ProfileType;     // 値が Type に変わる
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}
```

補助形式(C# 11+ ジェネリック属性):

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ByteMapperProfileAttribute<TProfile> : ByteMapperProfileAttribute
    where TProfile : class
{
    public ByteMapperProfileAttribute() : base(typeof(TProfile)) { }
}
```

- 主形式は `[ByteMapperProfile(typeof(SampleDataCodeNameProfile))]`。
- 補助形式 `[ByteMapperProfile<SampleDataCodeNameProfile>]` も同等に動く(継承による)。
- 内部ストレージ `HttpContext.Items[ByteMapperConst.ProfileKey]` の値型は **`Type`** に変更する(従来は `string`)。

### 2.4 Minimal API: 型パラメータで profile を指定する拡張(必須)

既存 `WithByteMapperBody<T>(string? profile = null)` / `WithByteMapperArrayBody<T>(string? profile = null)` の `profile` 引数を **削除** し、代わりに 2 型引数版を追加する:

```csharp
namespace Smart.IO.ByteMapper.AspNetCore.Filters;

public static class ByteMapperEndpointFilterExtensions
{
    // --- デフォルト Binding (profile なし) ---
    public static RouteHandlerBuilder WithByteMapperBody<TEntity>(this RouteHandlerBuilder builder);
    public static RouteHandlerBuilder WithByteMapperArrayBody<TEntity>(this RouteHandlerBuilder builder);

    // --- Profile 指定 Binding ---
    public static RouteHandlerBuilder WithByteMapperBody<TEntity, TProfile>(this RouteHandlerBuilder builder)
        where TProfile : class;
    public static RouteHandlerBuilder WithByteMapperArrayBody<TEntity, TProfile>(this RouteHandlerBuilder builder)
        where TProfile : class;
}
```

- メソッド名(`WithByteMapperBody` / `WithByteMapperArrayBody`)は据え置き、**型パラメータの数でプロファイル有無を表現** する。
- ユーザは `.WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>()` のように **1 行で完結** する。
- `typeof(TProfile)` をキーとして `registry.GetBinding<TEntity>(typeof(TProfile))` を引く(下記 2.5 参照)。
- リフレクション不使用。AOT 完全適合。

### 2.5 `ByteMapperRegistry` の API 変更(必須)

```csharp
namespace Smart.IO.ByteMapper.AspNetCore;

public sealed class ByteMapperRegistry
{
    private readonly FrozenDictionary<(Type Type, Type? Profile), ByteMapperBinding> singleBindings;
    private readonly FrozenDictionary<(Type Type, Type? Profile), object> arrayBindings;

    public ByteMapperRegistry(
        IReadOnlyDictionary<(Type, Type?), ByteMapperBinding> single,
        IReadOnlyDictionary<(Type, Type?), object> array)
    {
        singleBindings = single.ToFrozenDictionary();
        arrayBindings  = array.ToFrozenDictionary();
    }

    // --- 型パラメータ版(主用途) ---
    public ByteMapperBinding<T>? GetBinding<T>()
        => singleBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperBinding<T>? GetBinding<T, TProfile>() where TProfile : class
        => singleBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperBinding<T>)v : null;

    public ByteMapperArrayBinding<T>? GetArrayBinding<T>()
        => arrayBindings.TryGetValue((typeof(T), null), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public ByteMapperArrayBinding<T>? GetArrayBinding<T, TProfile>() where TProfile : class
        => arrayBindings.TryGetValue((typeof(T), typeof(TProfile)), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    // --- Type 引数版(Formatter のディスパッチ用) ---
    public ByteMapperBinding? GetBinding(Type entityType, Type? profileType = null)
        => singleBindings.TryGetValue((entityType, profileType), out var v) ? v : null;

    public object? GetArrayBinding(Type elementType, Type? profileType = null)
        => arrayBindings.TryGetValue((elementType, profileType), out var v) ? v : null;
}
```

- ジェネリック版に `T, TProfile` の 2 引数版を追加。
- 文字列 `profile` を受ける既存オーバーロードはすべて **削除**。

---

## 3. Generator 変更点(必須)

`Smart.IO.ByteMapper.AspNetCore.Generator/ByteMapperAspNetCoreGenerator.cs` の変更点:

### 3.1 `EndpointModel` の変更

```csharp
private sealed record EndpointModel(
    string Namespace,
    string ClassName,
    string EntityTypeFqn,
    string ReaderMethodName,
    string WriterMethodName,
    int Size,
    // string? ProfileKey,                 ← 削除
    string? ProfileTypeFqn,                  // ← 新規(プロファイルクラスの FQN, null なら デフォルト)
    bool GenerateArrayBinding,
    string RootNamespace);
```

### 3.2 `ParseEndpoint` の変更

`[ByteMapperEndpoint]` の `NamedArguments` から `Key` を読む処理を削除する。代わりに `[ByteReader]` / `[ByteWriter]` の `Profile = typeof(...)` を抽出する。

```csharp
// 既存ループ内で、ByteReader / ByteWriter 属性から Profile を取り出す
ITypeSymbol? profileType = null;

foreach (var member in classSymbol.GetMembers())
{
    if (member is not IMethodSymbol method || !method.IsStatic) continue;

    foreach (var attr in method.GetAttributes())
    {
        var name = attr.AttributeClass?.ToDisplayString();
        if (name != ByteReaderAttributeName && name != ByteWriterAttributeName) continue;

        foreach (var na in attr.NamedArguments)
        {
            if (na.Key == "Profile" && na.Value.Value is ITypeSymbol t)
            {
                profileType ??= t;
            }
        }
    }
}

// EndpointModel への投入
var profileFqn = profileType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
```

**整合性チェック(Diagnostic 追加)**:
- 同一クラス内の `[ByteReader]` と `[ByteWriter]` で異なる `Profile` を指定 → `SBM0030` (Error)
- `Profile` 指定があるが、その型が `[Map]` を持っていない → 既存 `SBM0013` に乗せる

### 3.3 Bootstrap 出力の変更

```csharp
// __ByteMapperAspNetCoreBootstrap.g.cs (生成出力)
var single = new Dictionary<(global::System.Type, global::System.Type?), global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding>
{
    { (typeof(global::Example.Web.Models.SampleData), typeof(global::Example.Web.Models.SampleDataCodeNameProfile)),
      global::Example.Web.Mappers.SampleDataCodeNameMappers.CreateByteMapperBinding() },
    { (typeof(global::Example.Web.Models.SampleData), null),
      global::Example.Web.Mappers.SampleDataMappers.CreateByteMapperBinding() },
    { (typeof(global::Example.Web.Models.SampleDataShort), null),
      global::Example.Web.Mappers.SampleDataShortMappers.CreateByteMapperBinding() },
};
```

- キータプルの第 2 要素が `string?` → **`global::System.Type?`** に変更。
- `null` はデフォルト(プロファイルなし)、それ以外は `typeof(...)` リテラル。
- 値型側は変更なし。

### 3.4 Generator 出力するファイル名

ファイル名は **エンティティ型 + Profile 型** から決定する。`SampleData_SampleDataCodeNameProfile` のような複合キーでもよいし、現状の「クラス名ベース」のまま続けても良い(`SampleDataCodeNameMappers` という命名で既に区別できているため)。**現状ベースの命名で十分**(変更不要)。

---

## 4. ランタイム側(`Smart.IO.ByteMapper.AspNetCore`)変更点(必須)

### 4.1 `ByteMapperConst.cs`

`ProfileKey` の値はそのまま(`"__ByteMapperProfile"`)。ただし、`HttpContext.Items` に格納する **値の型は `string` → `Type`** に変わる。コメントを修正:

```csharp
internal static class ByteMapperConst
{
    /// <summary>
    /// Key used to store the active ByteMapper profile <see cref="Type"/> in
    /// <see cref="Microsoft.AspNetCore.Http.HttpContext.Items"/>.
    /// Set by <see cref="Filters.ByteMapperProfileAttribute"/>.
    /// </summary>
    internal const string ProfileKey = "__ByteMapperProfile";
}
```

### 4.2 `Formatters/ByteMapperInputFormatter.cs` / `ByteMapperOutputFormatter.cs`

プロファイル取り出し箇所を `string` → `Type` に変更:

```csharp
// Before
var profile = httpContext.Items[ByteMapperConst.ProfileKey] as string;
var binding = (profile is not null ? registry.GetBinding(elementType, profile) : null)
              ?? registry.GetBinding(elementType);

// After
var profile = httpContext.Items[ByteMapperConst.ProfileKey] as Type;
var binding = (profile is not null ? registry.GetBinding(elementType, profile) : null)
              ?? registry.GetBinding(elementType);
```

`GetBinding(Type, Type?)` シグネチャに揃える。

### 4.3 `Filters/ByteMapperEndpointFilterExtensions.cs`

§2.4 の通り、`string? profile = null` 引数を全削除し、2 型引数版を追加:

```csharp
public static RouteHandlerBuilder WithByteMapperBody<TEntity>(this RouteHandlerBuilder builder)
    => AddFilter(builder, profileType: null);

public static RouteHandlerBuilder WithByteMapperBody<TEntity, TProfile>(this RouteHandlerBuilder builder)
    where TProfile : class
    => AddFilter(builder, profileType: typeof(TProfile));

// 内部実装(共通化)
private static RouteHandlerBuilder AddFilter<TEntity>(RouteHandlerBuilder builder, Type? profileType)
    => builder.AddEndpointFilterFactory((_, next) =>
        async invocationContext =>
        {
            var registry = invocationContext.HttpContext.RequestServices
                .GetRequiredService<ByteMapperRegistry>();
            var binding = registry.GetBinding(typeof(TEntity), profileType) as ByteMapperBinding<TEntity>
                ?? throw new InvalidOperationException(
                    $"No ByteMapperBinding registered for {typeof(TEntity).FullName} (profile={profileType?.FullName ?? "default"}).");

            await ReadIntoArgumentAsync(invocationContext, binding).ConfigureAwait(false);
            var result = await next(invocationContext).ConfigureAwait(false);
            if (result is TEntity typed)
            {
                return new ByteMapperResult<TEntity>(typed, binding);
            }
            return result;
        });
```

配列版 `WithByteMapperArrayBody` も同様。

### 4.4 `Filters/ByteMapperProfileAttribute.cs`

§2.3 の通り、`string profile` を取るコンストラクタを廃止し、`Type profileType` を取るコンストラクタに置き換え。

---

## 5. `Example.Web` 改修内容(必須)

### 5.1 `Program.cs`

```csharp
// 変更前
var registry = __ByteMapperAspNetCoreBootstrap.Build();
var formatterOptions = new ByteMapperFormatterOptions();
formatterOptions.SupportedMediaTypes.Add("text/x-fixedrecord");
builder.Services.AddSingleton(registry);
builder.Services.AddSingleton(formatterOptions);
builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Add(new ByteMapperOutputFormatter(registry, formatterOptions));
    options.InputFormatters.Add(new ByteMapperInputFormatter(registry, formatterOptions));
    options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
});
...
app.MapSampleEndpoints(registry);
```

```csharp
// 変更後
builder.Services.AddByteMapperFormatters(o =>                           // Generator 生成の拡張メソッド
{
    o.SupportedMediaTypes.Add("text/x-fixedrecord");
});

builder.Services.AddControllers(options =>
{
    // Formatter は DI から取り出して挿入
    options.FormatterMappings.SetMediaTypeMappingForFormat("dat", "text/x-fixedrecord");
});
builder.Services.Configure<MvcOptions>((sp, mvc) =>
{
    mvc.OutputFormatters.Insert(0, sp.GetRequiredService<ByteMapperOutputFormatter>());
    mvc.InputFormatters.Insert(0, sp.GetRequiredService<ByteMapperInputFormatter>());
});

...

app.MapSampleEndpoints();                                               // 引数なし
```

> 既存の Generator が生成している `__ByteMapperServiceCollectionExtensions.AddByteMapperFormatters` を活用することで、`Program.cs` は **`ByteMapperRegistry` という型名を 1 回も書かなく** て済む。

### 5.2 `Controllers/MapController.cs`

```csharp
// 変更前
public sealed class MapController(ByteMapperRegistry registry) : Controller
{
    [HttpGet]
    public IActionResult GetProfileCodeName()
    {
        var binding = registry.GetArrayBinding<SampleData>("code-name");
        ...                                                              // 30 行の手書きシリアライズ
    }
}
```

```csharp
// 変更後
public sealed class MapController : Controller                          // registry 注入不要
{
    [Produces("text/x-fixedrecord")]
    [HttpGet]
    [ByteMapperProfile(typeof(SampleDataCodeNameProfile))]              // ← 属性で profile 指定
    public SampleData[] GetProfileCodeName() => CreateDummyData();      // 1 行

    [HttpPost]
    [ByteMapperProfile(typeof(SampleDataCodeNameProfile))]
    public IActionResult PostProfileCodeName([FromBody] SampleData[] values)
        => Ok(new { count = values.Length });
}
```

### 5.3 `MinimalApi/SampleEndpoints.cs`

```csharp
// 変更後
public static void MapSampleEndpoints(this WebApplication app)          // registry 引数不要
{
    var group = app.MapGroup("/minimal/sample");

    // 既存のデフォルトプロファイルは無変更
    group.MapGet("/array", () => CreateDummyData())
        .WithByteMapperArrayBody<SampleData>();
    group.MapPost("/array", (SampleData[] values)
            => Results.Ok(new { count = values.Length }))
        .WithByteMapperArrayBody<SampleData>();

    // プロファイル指定(型パラメータで指定)
    group.MapGet("/profile/code-name", () => CreateDummyData())
        .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>();

    group.MapPost("/profile/code-name", (SampleData[] values)
            => Results.Ok(new { count = values.Length }))
        .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>();
}
```

- `(HttpContext ctx)` でゴリゴリ手書きしていた `MapPost` がフレームワーク側に吸い込まれ、ハンドラ本体は **デフォルト経路と同形** になる。
- `ReadArrayAsync` ヘルパは Filter 側で吸収済みのため、`Example.Web` から削除。

### 5.4 `Mappers/SampleDataCodeNameMappers.cs`

```csharp
// 変更前
[ByteMapperEndpoint(Key = "code-name")]                                 // ← Key プロパティ
public static partial class SampleDataCodeNameMappers { ... }
```

```csharp
// 変更後
[ByteMapperEndpoint]                                                    // ← Key 削除
public static partial class SampleDataCodeNameMappers
{
    [ByteReader(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter(Profile = typeof(SampleDataCodeNameProfile))]
    public static partial void Write(SampleData source, Span<byte> destination);
}
```

クラス自体の存在(`SampleDataCodeNameMappers` という名前)から識別性は十分。Profile 型は `[ByteReader]/[ByteWriter]` に既に明示されている。

---

## 6. 設計選択肢の比較(参考)

実装に着手する前に検討した主要パターンを記録しておく。

### 6.1 プロファイル識別子: 文字列 vs 型

| 案 | 識別子 | Pros | Cons | 採否 |
| --- | --- | --- | --- | --- |
| A. 文字列 Key 維持 | `string Key` | 文字列リテラルで URL ライク, 旧 API との一貫性 | タイポ無検出, 二重記述, ID と型の同期はユーザ責任 | 不採用 |
| **B. Profile 型** | `Type ProfileType` | コンパイル時チェック, IntelliSense 補完, `[ByteReader(Profile=typeof(X))]` と単一情報源 | 既存 API 利用者の破壊変更 | **採用** |
| C. 列挙型 / 定数クラス | `enum ProfileId` | 文字列より型安全 | プロファイル追加時に enum 更新が必要, Profile 型との二重定義 | 不採用 |

性能比較(100 万件 × ルックアップ):
- A 案: `FrozenDictionary<(Type, string?), _>.TryGetValue` — string ハッシュ + equality (CPU キャッシュ次第)
- B 案: `FrozenDictionary<(Type, Type?), _>.TryGetValue` — `Type` は参照同一性なので **`RuntimeHelpers.GetHashCode` + 参照比較**。文字列より高速で安定。
- 差分は微小だが、ホットパスでは **B 案が等価以上**。

### 6.2 Minimal API のプロファイル指定形式

| 案 | シグネチャ | Pros | Cons | 採否 |
| --- | --- | --- | --- | --- |
| α. 文字列引数 | `.WithByteMapperBody<T>("code-name")` | 簡単 | タイポ無検出, 文字列リテラル | 不採用 |
| **β. 2 型引数** | `.WithByteMapperBody<T, TProfile>()` | コンパイル時チェック, シンプル, リフレクション不要 | C# のオーバーロード解決ルールに沿う必要あり | **採用** |
| γ. メソッド名差分 | `.WithCodeNameProfile()` | プロファイル毎に専用名 | Generator がユーザアセンブリに拡張を生成する必要あり | 不採用 |
| δ. マーカークラス | `.WithByteMapperBody<T>(default(SampleDataCodeNameProfile))` | 型引数推論できる | 値型の `default` を引数で渡す悪手, 視認性低 | 不採用 |

性能比較:
- α は `string` を一度キャプチャしてクロージャに保持。
- β は `typeof(TProfile)` がコンパイル時定数として埋め込まれ、クロージャすら発生しない可能性が高い(JIT/AOT 双方で最適化)。
- **β > α** が明確。

### 6.3 MVC 属性形式

| 案 | 形式 | Pros | Cons | 採否 |
| --- | --- | --- | --- | --- |
| i. 文字列 | `[ByteMapperProfile("code-name")]` | 既存形式 | 6.1 と同じ問題 | 不採用 |
| **ii. typeof 引数** | `[ByteMapperProfile(typeof(SampleDataCodeNameProfile))]` | 全 C# バージョン対応, 主流 | やや冗長 | **主形式採用** |
| iii. ジェネリック属性 | `[ByteMapperProfile<SampleDataCodeNameProfile>]` | 最も簡潔, タイプセーフ | C# 11+ 必須 | **補助形式として併設** |

採用: ii を主、iii を補助(ii を継承する形で実装)。

---

## 7. 性能評価

### 7.1 ルックアップコスト

`FrozenDictionary<(Type, Type?), _>` のルックアップは O(1) 級だが、キーが `ValueTuple<Type, Type?>` のため等値判定は:
- `Type.Equals(Type)` → 参照同一性 (`RuntimeType` のため `RuntimeHelpers.Equals` レベル)
- `Type?.Equals(Type?)` → null 同士でも安全に分岐

旧 `(Type, string?)` の場合、文字列比較は `Ordinal` の場合でも N 回比較が走る。`Type` 比較は単一参照同一性チェックで完結。**B 案(Type キー)の方が高速**。

### 7.2 ホットパスへの影響

ルックアップは **MVC Formatter が呼ばれるたび** に 1 回、**Minimal API Filter のリクエスト毎** に 1 回発生する。100 万 req/s 級のシナリオで:
- 旧: `Type` ハッシュ + `string` ハッシュ + `string Equals` 比較 (≈ 10–20 ns)
- 新: `Type` ハッシュ + `Type` ハッシュ + 参照比較 (≈ 5–10 ns)

「測定誤差レベルの改善」と「タイプセーフ確保」の両取り。

### 7.3 メモリ

- 旧: 文字列インスタンス `"code-name"` を 1 個保持 (≈ 24 bytes + chars)
- 新: `typeof(SampleDataCodeNameProfile)` の `Type` 参照のみ。`Type` インスタンスは CLR が常に保持しているため追加コスト 0。

新方式の方が **若干の常駐メモリ削減**。

### 7.4 AOT 適合性

- `typeof(T)` はコンパイル時定数 → AOT 完全適合。
- `MakeGenericType` 不使用。
- リフレクション不使用。

旧設計の AOT 適合性は維持しつつ、API 表現力を上げる。

---

## 8. 実装手順(チェックリスト)

実装は以下の順序で進めること。各ステップはテスト可能な独立単位とする。

1. [ ] `Smart.IO.ByteMapper.AspNetCore/ByteMapperRegistry.cs` のキーを `(Type, string?)` → `(Type, Type?)` に変更。
2. [ ] `Smart.IO.ByteMapper.AspNetCore/ByteMapperEndpointAttribute.cs` から `Key` プロパティ削除。
3. [ ] `Smart.IO.ByteMapper.AspNetCore.Generator/ByteMapperAspNetCoreGenerator.cs`:
   - [ ] `EndpointModel` の `ProfileKey` を `ProfileTypeFqn` に変更。
   - [ ] `ParseEndpoint` で `[ByteReader]/[ByteWriter]` から `Profile = typeof(...)` を抽出。
   - [ ] `BuildBootstrapSource` でキーを `typeof(...)` リテラルとして emit。
   - [ ] `[ByteReader]` と `[ByteWriter]` の Profile 不整合 Diagnostic (`SBM0030`) 追加。
4. [ ] `Smart.IO.ByteMapper.AspNetCore/Formatters/ByteMapperInputFormatter.cs` / `ByteMapperOutputFormatter.cs`:
   - [ ] `HttpContext.Items[ByteMapperConst.ProfileKey] as string` → `as Type`。
   - [ ] `registry.GetBinding(Type, string?)` → `GetBinding(Type, Type?)` 呼び出しに揃える。
5. [ ] `Smart.IO.ByteMapper.AspNetCore/Filters/ByteMapperProfileAttribute.cs`:
   - [ ] コンストラクタを `Type profileType` に変更。
   - [ ] `HttpContext.Items` に `Type` を格納するよう変更。
6. [ ] `Smart.IO.ByteMapper.AspNetCore/Filters/ByteMapperProfileAttribute.cs` に `ByteMapperProfileAttribute<TProfile> : ByteMapperProfileAttribute` を追加(ジェネリック属性版)。
7. [ ] `Smart.IO.ByteMapper.AspNetCore/Filters/ByteMapperEndpointFilterExtensions.cs`:
   - [ ] `string? profile = null` 引数を全削除。
   - [ ] `WithByteMapperBody<TEntity, TProfile>()` / `WithByteMapperArrayBody<TEntity, TProfile>()` を追加。
   - [ ] 内部実装を `Type?` ベースに統一。
8. [ ] `Example.Web/Mappers/SampleDataCodeNameMappers.cs` から `Key = "code-name"` を削除。
9. [ ] `Example.Web/Controllers/MapController.cs`:
   - [ ] `ByteMapperRegistry registry` パラメータ削除。
   - [ ] `GetProfileCodeName` / `PostProfileCodeName` を `[ByteMapperProfile(typeof(...))]` 属性 + Formatter 経由に書き換え。
10. [ ] `Example.Web/MinimalApi/SampleEndpoints.cs`:
    - [ ] `ByteMapperRegistry` 引数削除。
    - [ ] `(HttpContext ctx)` 手書きハンドラを Filter ベースに置き換え。
    - [ ] `ReadArrayAsync` ヘルパを削除。
11. [ ] `Example.Web/Program.cs`:
    - [ ] `__ByteMapperAspNetCoreBootstrap.Build()` の直接呼び出しを削除。
    - [ ] `builder.Services.AddByteMapperFormatters(...)` に切り替え。
    - [ ] `MapSampleEndpoints(registry)` → `MapSampleEndpoints()`。
12. [ ] `Smart.IO.ByteMapper.Tests` に以下のテストを追加:
    - [ ] `GetBinding<T>()` / `GetBinding<T, TProfile>()` の戻り値検証。
    - [ ] `[ByteMapperProfile(typeof(...))]` 経由で Formatter が正しいバインディングを引くこと。
    - [ ] `WithByteMapperBody<T, TProfile>()` が正しいバインディングで body を読み書きすること。
    - [ ] 同一クラス内 `[ByteReader]/[ByteWriter]` の Profile 不整合で `SBM0030` が発火すること。
13. [ ] `Docs/AspNetCore.md` 第 7 章「Profile 対応」を新 API に追従して更新(または参照リンクを本書に張る)。
14. [ ] ビルド警告ゼロを維持(AGENTS.md 規約)。

---

## 9. 受け入れ基準

以下を満たすこと:

- [ ] `Example.Web` 内のソースコードから `ByteMapperRegistry` という型名の出現が **`Program.cs` 以外で 0 回**。
   - `Program.cs` でも理想は 0 回(`AddByteMapperFormatters()` 拡張で隠蔽)。
- [ ] `Example.Web` 内のソースコードから `"code-name"` のような **プロファイル文字列リテラルが 0 回**。
- [ ] `MapController` のコンストラクタ依存性が **0 個**(`Controller` 既定のみ)。
- [ ] `MapSampleEndpoints(this WebApplication app)` の **引数が `app` のみ**。
- [ ] 既存 E2E テスト(HTTP リクエスト経由のシリアライズ確認)がすべて緑。
- [ ] AOT パブリッシュ (`Smart.IO.ByteMapper.AotTest`) でビルド警告 0 / 実行成功。
- [ ] BenchmarkDotNet 比較(`ByteMapperFormatter` 経由 vs 旧手書き)で **性能劣化なし**(同等または上回る)。

---

## 10. 残課題・要決定事項

1. **複数 Profile 同型登録**: 同じ `(SampleData, SampleDataCodeNameProfile)` を別マッパクラスで登録 → 重複登録エラー Diagnostic (`SBM0031`) を追加するか。当面は最初の 1 個採用 + Warning とする案を推奨。
2. **デフォルト + Profile 両立の優先順位**: Formatter 内のフォールバック(`profile が見つからなければデフォルトを引く`)は維持するか、それとも Profile 指定があれば必ず Profile を使い、無ければエラーとするか。**現行のフォールバック挙動を維持** することを推奨(ロバスト性)。
3. **`AddByteMapperFormatters` 内での MVC formatter 挿入**: 現在は手動で `mvc.InputFormatters.Insert(0, ...)` を書く必要がある。これも Generator 生成の拡張で自動化するか。本書のスコープ外として将来検討。
4. **`[ByteMapperEndpoint]` への `Profile` プロパティ昇格**: `[ByteReader]/[ByteWriter]` 個別で Profile を書く現状を、クラス全体 1 箇所に集約する案。基本同じ Profile しか使わないため UX 向上。本書のスコープ外として将来検討。
5. **OpenAPI / Swagger 連携**: `[ByteMapperProfile]` 属性付き action のレスポンス形式を OpenAPI スキーマに反映する補助。本書のスコープ外。

---

## 11. まとめ

- プロファイル識別子を **文字列 → Profile クラスの `Type`** に統一する。
- MVC は `[ByteMapperProfile(typeof(TProfile))]` 属性で、Minimal API は `WithByteMapperBody<TEntity, TProfile>()` の **型パラメータ** で profile を指定する。
- `Example.Web` の `MapController` / `SampleEndpoints` から `ByteMapperRegistry` 直接利用を **完全に排除** する。
- 性能はわずかに向上(string キー比較 → Type 参照比較)、AOT 適合性は維持、API の型安全性が大幅向上。
- まだリリース前の API のため、破壊的変更を許容して **最終形態に到達** させる。
