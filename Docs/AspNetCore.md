# Smart.IO.ByteMapper.AspNetCore (Source Generator 版) 設計検討書

最終更新: 2026-05-21

本書は、Source Generator 化に伴って `Smart.IO.ByteMapper.AspNetCore` がどのように再構築されるべきかを検討した **方針案** をまとめたものである。本フェーズでは確定仕様ではなく、複数案の比較と推奨アプローチを示す。

---

## 1. 目的・スコープ

### 1.1 目的
- 既存 `Smart.IO.ByteMapper.AspNetCore` が提供している ASP.NET Core 連携機能を、AOT/Trim 適合の形で再実装する方針を示す。
- 旧 API 利用者の移行ガイドの基礎情報とする。

### 1.2 対象機能(既存版で提供されているもの)
| 機能 | 旧クラス |
| --- | --- |
| MVC `IInputFormatter` | `ByteMapperInputFormatter` |
| MVC `IOutputFormatter` | `ByteMapperOutputFormatter` |
| Minimal API `IEndpointFilter` | `ByteMapperEndpointFilter` |
| Profile 指定 | `ByteMapperProfileAttribute`(`IAuthorizationFilter`)+ `HttpContext.Items["__ByteMapperProfile"]` |
| 設定 | `ByteMapperFormatterConfig`(`MapperFactory` / `IDelegateFactory` / 対応 MediaType / BufferSize) |
| 単一 / 配列 / `IEnumerable<T>` 入出力 | `SingleInputReader<T>` / `ArrayInputReader<T>` / `ListInputReader<T>` / `SingleOutputWriter<T>` / `EnumerableOutputWriter<T>` |

### 1.3 非ゴール(本フェーズ)
- `Microsoft.AspNetCore.Mvc.Versioning` 等の周辺連携。
- gRPC / SignalR 連携(必要なら別フェーズ)。
- 旧版バイナリ互換(SemVer メジャー更新を許容する)。

---

## 2. 既存実装の AOT 非適合点(問題分析)

旧 `ByteMapperInputFormatter` / `ByteMapperOutputFormatter` / `ByteMapperEndpointFilter` には、Native AOT で警告/失敗となる依存が複数存在する。

| 非適合箇所 | 旧コードでの呼び出し | 影響 |
| --- | --- | --- |
| **リフレクション型構築** | `typeof(SingleInputReader<>).MakeGenericType(elementType)` 等 | `RequiresDynamicCode` 警告。AOT で `MakeGenericType` は事前生成されていない型に対しては失敗する。 |
| **コンストラクタ呼び出し** | `type.GetConstructor([typeof(ByteMapperFormatterConfig), typeof(string)])!.Invoke([...])` | `RequiresUnreferencedCode` 警告。Trim で削除される可能性。 |
| **`MapperFactory.Create(Type)` / `Create<T>()`** | 旧 `Smart.IO.ByteMapper` がリフレクション + 動的型生成で `ITypeMapper<T>` を構築 | Source Generator 版では存在しない API。新版では `static partial` メソッドが直接呼ばれる構造に変更されているため、`MapperFactory` のような型ベースのルックアップは作れない。 |
| **`IDelegateFactory.CreateFactory<T>()`** | リフレクションで `Func<T>` を構築 | AOT 非適合。Source Generator では `new T()` を直書きできるため不要。 |
| **`(T)model` キャスト + `IEnumerable<T>` キャスト** | `model.GetType()` から動的に readers/writers を解決 | キャスト自体は問題ないが、解決経路にリフレクションが含まれるため経路全体が非適合となる。 |
| **`object` 経由のコレクション操作** | `EnumerableOutputWriter<T>` で `(IEnumerable<T>)model` を foreach | boxing は無いが、`Type` キーでのディスパッチがリフレクションに依存。 |
| **MVC `OutputFormatter.CanWriteResult` のデフォルト挙動** | `OutputFormatter` が ContentType マッチで型を素通しする前提 | Trim で型情報が消えると正しく動作しない場合がある。 |

要点: **「Type → Mapper の解決」をリフレクションなしで成立させること** が最大の設計課題である。

---

## 3. 設計方針

### 3.1 基本方針
1. **Type → Mapper の解決テーブルは Source Generator が事前生成する**。リフレクションでの型構築・コンストラクタ呼び出しを完全に排除する。
2. **MVC formatter / Minimal API filter は型情報を「事前登録された Binding」経由で解決する**。`Activator.CreateInstance` / `MakeGenericType` は使わない。
3. **ユーザは登録ポイントを 1 箇所で宣言する**(`builder.Services.AddByteMapperFormatters()` などの拡張メソッド呼び出しで完結させる)。
4. **Minimal API 向けには、強い型付きの拡張メソッドを優先**(`.WithByteMapperBody<OrderRecord>()`)。リフレクションを介さず引数型を直接指定する。
5. **配列 / `IEnumerable<T>` の取り扱いも Generator 側で各要素型ごとに専用バインディングを生成**する(`MakeGenericType` を避ける)。

### 3.2 名前空間と Assembly 構成
| プロジェクト | TFM | 役割 |
| --- | --- | --- |
| `Smart.IO.ByteMapper.AspNetCore` | `net8.0;net9.0;net10.0` | ランタイム(Formatter / Filter / Binding 基底)。`Smart.IO.ByteMapper` に依存。 |
| (Source Generator は `Smart.IO.ByteMapper.Generator` に**統合**する想定 ── 詳細は §10 で議論) | — | — |

### 3.3 ユーザ API の最終形(目標イメージ)

```csharp
// Program.cs
var builder = WebApplication.CreateSlimBuilder(args);

// ① レジストリ初期化(Generator が拡張メソッドを生成)
builder.Services.AddByteMapperFormatters(static options =>
{
    options.SupportedMediaTypes.Add("application/octet-stream");
    options.BufferSize = 8192;
    // 登録は Generator 生成のヘルパが行う(下記 §4 参照)
});

// MVC 利用
builder.Services.AddControllers(static mvc =>
{
    mvc.InputFormatters.Insert(0, ByteMapperFormatters.CreateInputFormatter());
    mvc.OutputFormatters.Insert(0, ByteMapperFormatters.CreateOutputFormatter());
});

// Minimal API 利用
var app = builder.Build();
app.MapPost("/orders", static (OrderRecord r) => r)
   .WithByteMapperBody<OrderRecord>();
app.MapPost("/orders/array", static (OrderRecord[] r) => r)
   .WithByteMapperBody<OrderRecord[]>();
```

---

## 4. 提案アーキテクチャ

### 4.1 中核となる Binding 型

ランタイムが各型に対して持つ「Read/Write 関数 + サイズ」のセットを `ByteMapperBinding<T>` として公開する。リフレクションを使わずデリゲートを直書きで保持する。

```csharp
namespace Smart.IO.ByteMapper.AspNetCore;

// 単一エンティティ用バインディング
public sealed class ByteMapperBinding<T>
{
    public int Size { get; }
    public ReadDelegate Read { get; }
    public WriteDelegate Write { get; }
    public FactoryDelegate Factory { get; }   // T Read(ReadOnlySpan<byte>) で使う

    public ByteMapperBinding(int size, ReadDelegate read, WriteDelegate write, FactoryDelegate factory)
    {
        Size = size;
        Read = read;
        Write = write;
        Factory = factory;
    }

    public delegate void ReadDelegate(ReadOnlySpan<byte> source, T target);
    public delegate void WriteDelegate(T source, Span<byte> destination);
    public delegate T FactoryDelegate();
}

// 配列/コレクション用バインディング(要素サイズ + 要素 Read/Write を保持)
public sealed class ByteMapperArrayBinding<T>
{
    public int ElementSize { get; }
    public ByteMapperBinding<T>.ReadDelegate ReadElement { get; }
    public ByteMapperBinding<T>.WriteDelegate WriteElement { get; }
    public ByteMapperBinding<T>.FactoryDelegate Factory { get; }

    public ByteMapperArrayBinding(
        int elementSize,
        ByteMapperBinding<T>.ReadDelegate readElement,
        ByteMapperBinding<T>.WriteDelegate writeElement,
        ByteMapperBinding<T>.FactoryDelegate factory)
    {
        ElementSize = elementSize;
        ReadElement = readElement;
        WriteElement = writeElement;
        Factory = factory;
    }
}
```

ユーザは Source Generator 版の `[ByteReader]`/`[ByteWriter]` partial メソッドを書いた上で、AspNetCore 層に対しては Binding を提供する。Binding はユーザが手書きしてもよいが、後述の `[ByteMapperEndpoint]` 属性を使えば **Generator が自動生成** する。

### 4.2 Type ↔ Binding レジストリ

実行時には Type をキーとして Binding を取り出す必要があるため、`Dictionary<Type, object>` 相当のレジストリを持つ。**ただし AOT で安全な FrozenDictionary** を採用し、初期化後は変更しない。

```csharp
namespace Smart.IO.ByteMapper.AspNetCore;

public sealed class ByteMapperRegistry
{
    private readonly System.Collections.Frozen.FrozenDictionary<Type, object> singleBindings;
    private readonly System.Collections.Frozen.FrozenDictionary<Type, object> arrayBindings;

    public ByteMapperRegistry(
        System.Collections.Generic.IReadOnlyDictionary<Type, object> single,
        System.Collections.Generic.IReadOnlyDictionary<Type, object> array)
    {
        singleBindings = single.ToFrozenDictionary();
        arrayBindings = array.ToFrozenDictionary();
    }

    public ByteMapperBinding<T>? GetBinding<T>()
        => singleBindings.TryGetValue(typeof(T), out var v) ? (ByteMapperBinding<T>)v : null;

    public object? GetBinding(Type type)
        => singleBindings.TryGetValue(type, out var v) ? v : null;

    public ByteMapperArrayBinding<T>? GetArrayBinding<T>()
        => arrayBindings.TryGetValue(typeof(T), out var v) ? (ByteMapperArrayBinding<T>)v : null;

    public object? GetArrayBinding(Type elementType)
        => arrayBindings.TryGetValue(elementType, out var v) ? v : null;
}
```

要点:
- `FrozenDictionary` は `net8.0+` でゼロアロケーション・読み込み専用キャッシュとして AOT 適合。
- レジストリは `IServiceProvider` から取得され、Formatter/Filter で共有される。
- **`MakeGenericType` を一切使わず**、Generator が生成する初期化コードで具体的な閉じた型 `ByteMapperBinding<OrderRecord>` を直接 `new` して投入する。

### 4.3 ユーザ宣言と Generator による Binding 生成

#### 4.3.1 既存の `[ByteReader]`/`[ByteWriter]` を使った宣言(基本)

```csharp
// 既存(Source Generator 本体が生成する)
internal static partial class OrderMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);

    [ByteWriter]
    public static partial void Write(OrderRecord source, Span<byte> buffer);
}
```

#### 4.3.2 AspNetCore 向け追加属性(新規)

```csharp
namespace Smart.IO.ByteMapper.AspNetCore;

/// <summary>
/// この属性が付いた partial method 群を持つクラスを「AspNetCore レジストリのエントリ点」として扱う。
/// Generator が同クラスに ByteMapperBinding 提供 / 登録メソッドを生成する。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class ByteMapperEndpointAttribute : Attribute
{
    /// <summary>登録対象とする Read/Write メソッドのペアを識別するメソッド名(共通名)。省略時は同クラスの [ByteReader]/[ByteWriter] 全てを対象とする。</summary>
    public string? MethodName { get; init; }
    /// <summary>配列対応を生成するか</summary>
    public bool GenerateArrayBinding { get; init; } = true;
}
```

ユーザ宣言例:

```csharp
[ByteMapperEndpoint]
internal static partial class OrderMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);

    [ByteReader]
    public static partial OrderRecord Read(ReadOnlySpan<byte> buffer);

    [ByteWriter]
    public static partial void Write(OrderRecord source, Span<byte> buffer);
}
```

Generator が emit するもの(イメージ):

```csharp
// OrderMappers.AspNetCore.g.cs (Generator 出力)
partial class OrderMappers
{
    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding<global::MyApp.OrderRecord> CreateBinding()
        => new(
            size: 40,                       // ← Map(Size) から
            read:  Read,                    // ← partial メソッドを直参照(リフレクション無し)
            write: Write,
            factory: static () => new global::MyApp.OrderRecord());

    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperArrayBinding<global::MyApp.OrderRecord> CreateArrayBinding()
        => new(
            elementSize: 40,
            readElement:  Read,
            writeElement: Write,
            factory:      static () => new global::MyApp.OrderRecord());
}
```

#### 4.3.3 DI 登録ヘルパ

Generator は更にアセンブリレベルで **全エントリ点をまとめて登録するヘルパ** を 1 つ生成する:

```csharp
// __ByteMapperRegistration.g.cs (Generator 出力)
namespace Smart.IO.ByteMapper.AspNetCore.Generated;

public static class ByteMapperRegistration
{
    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry CreateRegistry()
    {
        var single = new global::System.Collections.Generic.Dictionary<global::System.Type, object>
        {
            { typeof(global::MyApp.OrderRecord), global::MyApp.OrderMappers.CreateBinding() },
            { typeof(global::MyApp.CustomerRecord), global::MyApp.CustomerMappers.CreateBinding() },
            // … 他のエントリ点
        };
        var array = new global::System.Collections.Generic.Dictionary<global::System.Type, object>
        {
            { typeof(global::MyApp.OrderRecord), global::MyApp.OrderMappers.CreateArrayBinding() },
            // …
        };
        return new global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry(single, array);
    }
}
```

ユーザ側は単に拡張メソッド 1 行で済む:

```csharp
builder.Services.AddByteMapperFormatters();   // 内部で CreateRegistry() を呼ぶ
```

ランタイム側 `AddByteMapperFormatters` 拡張メソッドは、Generator 生成の `CreateRegistry()` を呼んで `ByteMapperRegistry` を Singleton 登録する。**`AddByteMapperFormatters` の実装には Generator 生成型への参照を含めるため、AspNetCore ランタイムアセンブリは「Generator が同居するアセンブリ」を直参照する。**(あるいは中継として薄い `IByteMapperBootstrapper` インタフェースを公開し、Generator がそれを実装する。)

---

## 5. MVC Formatter 設計

### 5.1 `ByteMapperInputFormatter`(新版)

```csharp
namespace Smart.IO.ByteMapper.AspNetCore.Formatters;

public sealed class ByteMapperInputFormatter : InputFormatter
{
    private readonly ByteMapperRegistry registry;
    private readonly ByteMapperFormatterOptions options;

    public ByteMapperInputFormatter(ByteMapperRegistry registry, ByteMapperFormatterOptions options)
    {
        this.registry = registry;
        this.options  = options;
        foreach (var m in options.SupportedMediaTypes)
        {
            SupportedMediaTypes.Add(m);
        }
    }

    protected override bool CanReadType(Type type)
        => registry.GetBinding(type) is not null
        || (IsArrayOrList(type, out var elem) && registry.GetArrayBinding(elem!) is not null);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var modelType = context.ModelType;

        if (IsArrayOrList(modelType, out var elementType))
        {
            var arrayBinding = registry.GetArrayBinding(elementType!);
            if (arrayBinding is null) return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
            var result = await ReadArrayAsync(arrayBinding, elementType!, context).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }

        var binding = registry.GetBinding(modelType);
        if (binding is null) return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
        var single = await ReadSingleAsync(binding, modelType, context).ConfigureAwait(false);
        return await InputFormatterResult.SuccessAsync(single).ConfigureAwait(false);
    }

    // 実装: 型ごとに dispatch するヘルパは Generic ではない object を介す経路にせざるを得ない。
    // ただし「Type → Binding」の解決は事前生成のため、MakeGenericType は使わない。
    // 個別 dispatch は IByteMapperDispatcher のような薄い経路を Generator が emit する案がある(§5.3 参照)。
}
```

### 5.2 `ByteMapperOutputFormatter`(新版)

同様に `OutputFormatter` を継承し、`context.ObjectType` → Binding ルックアップで分岐する。Single / Array / `IEnumerable<T>` の判別は **配列かどうか** または **`IEnumerable<T>` 実装かどうか** で行うが、その判定だけは `Type` API レベルで済む(後者は `typeof(IEnumerable<>).MakeGenericType` を **避けて** 既知ジェネリック構造のみを許可)。

### 5.3 Type 経由ディスパッチの解決方法

`registry.GetBinding(Type)` で取れる Binding は **`object`** 型である(`Dictionary<Type, object>`)。実際の `ByteMapperBinding<T>` への安全なキャストには 2 案ある。

| 案 | 概要 | 採否 |
| --- | --- | --- |
| **A. ディスパッチデリゲートも Binding に同梱** | `ByteMapperBinding` 抽象基底 + `ByteMapperBinding<T>` 派生。基底に `ReadAsObject(span) → object` / `WriteAsObject(object, span)` の **非ジェネリック経路** を生やす(boxing は単一要素では 1 回のみ) | **採用候補** |
| B. `Type` ごとに専用 Formatter を生成 | Generator が `OrderRecord` 用 / `CustomerRecord` 用の専用 Formatter クラスを emit。`Microsoft.AspNetCore.Mvc.Formatters.IInputFormatter` を実装。MVC のフォーマッタ列挙コストが増える代わりに `object` 経由を完全排除 | 性能優位だが運用負担増 |
| C. ジェネリック型をリストし、ランタイムでパターンマッチ | 旧版に近い動き。`MakeGenericType` 不要にできれば検討余地あり | 困難 |

**A 案を採用**する場合:
```csharp
public abstract class ByteMapperBinding
{
    public abstract int Size { get; }
    public abstract object Factory();
    public abstract void Read(ReadOnlySpan<byte> source, object target);
    public abstract void Write(object source, Span<byte> destination);
}

public sealed class ByteMapperBinding<T> : ByteMapperBinding
{
    // 非ジェネリック側はキャスト経由で呼ばれる(単発呼び出しで boxing 影響軽微)
    public override void Read(ReadOnlySpan<byte> source, object target)
        => readDelegate(source, (T)target);
    public override void Write(object source, Span<byte> destination)
        => writeDelegate((T)source, destination);
    // ジェネリック版もそのまま公開: Read(ReadOnlySpan<byte>, T)
}
```

- 単一エンティティの Read/Write では `(T)target` キャスト 1 回のみで、boxing は値型のみ問題となる(参照型なら完全に無コスト)。
- 値型固定長レコードを送受信する用途では boxing がボトルネックになりうる → その場合は **B 案(専用 Formatter)** を選択可能にする(`[ByteMapperEndpoint(DedicatedFormatter = true)]`)。

---

## 6. Minimal API Filter 設計

### 6.1 強型版 `WithByteMapperBody<T>()`(推奨)

```csharp
namespace Smart.IO.ByteMapper.AspNetCore.Filters;

public static class ByteMapperEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithByteMapperBody<T>(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilterFactory(static (factoryContext, next) =>
        {
            // 起動時に Binding を解決(リフレクションなし: typeof(T) で直接)
            return async invocationContext =>
            {
                var sp = invocationContext.HttpContext.RequestServices;
                var registry = sp.GetRequiredService<ByteMapperRegistry>();
                var binding = registry.GetBinding<T>()
                    ?? throw new InvalidOperationException($"No ByteMapperBinding registered for {typeof(T)}");

                // body を読み取り T に変換し、第 1 引数(または T 型引数)に差し込む
                var target = await ReadAsync(invocationContext.HttpContext, binding).ConfigureAwait(false);
                ReplaceArgument(invocationContext, target);

                var result = await next(invocationContext).ConfigureAwait(false);
                if (result is T typed)
                {
                    return new ByteMapperResult<T>(typed, binding);
                }
                return result;
            };
        });

    public static RouteHandlerBuilder WithByteMapperBody<T>(this RouteHandlerBuilder builder) where T : ... { /* 配列版は別 overload */ }
}
```

ポイント:
- **`typeof(T)` でレジストリ参照する** ため `MakeGenericType` は不要。
- 引数差し替えは `EndpointFilterInvocationContext.Arguments` をスキャンして `T` 型 / `T[]` 型 / `IEnumerable<T>` 型のスロットに代入する。スキャンは型一致比較のみで AOT 安全。

### 6.2 弱型版 `WithByteMapperFilter()`(レガシー互換)

旧版の挙動互換が必要な場合のみ提供する。引数の `GetType()` から `registry.GetBinding(Type)` で探し、見つかれば差し替える形。`MakeGenericType` を呼ばないので AOT 適合は維持できるが、性能は強型版より劣る。

### 6.3 配列対応

```csharp
app.MapPost("/orders/array", static (OrderRecord[] r) => r)
   .WithByteMapperArrayBody<OrderRecord>();   // T = 要素型を指定
```

Generator は要素型 `OrderRecord` から `ByteMapperArrayBinding<OrderRecord>` を提供し、Filter は body を読み込んで `OrderRecord[]` を構築する。`List<T>` 版は `WithByteMapperListBody<OrderRecord>()` のように別拡張で提供する。

---

## 7. Profile 対応

### 7.1 旧版の方式
- `[ByteMapperProfile("v1")]` を Controller/Action に付け、`IAuthorizationFilter` で `HttpContext.Items["__ByteMapperProfile"]` に文字列をセット。
- Formatter 側で `MapperFactory.Create<T>(profile)` を呼んで Profile 別 Mapper を取得。

### 7.2 新版の方針
- 旧版の **文字列プロファイル名 → Mapper** という間接参照は廃止する(リフレクション必須のため)。
- 代わりに **Profile ごとに別の partial メソッド群** を宣言させ、Profile ごとに **異なる Binding** をユーザがレジストリ登録する。

```csharp
[ByteMapperEndpoint(Key = "v1")]
internal static partial class OrderMappersV1
{
    [ByteReader(Profile = typeof(OrderProfileV1))] public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);
    [ByteWriter(Profile = typeof(OrderProfileV1))] public static partial void Write(OrderRecord source, Span<byte> buffer);
}

[ByteMapperEndpoint(Key = "v2")]
internal static partial class OrderMappersV2
{
    [ByteReader(Profile = typeof(OrderProfileV2))] public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);
    [ByteWriter(Profile = typeof(OrderProfileV2))] public static partial void Write(OrderRecord source, Span<byte> buffer);
}
```

レジストリ側は `(Type, ProfileKey)` の複合キーをサポートする。

```csharp
public sealed class ByteMapperRegistry
{
    private readonly FrozenDictionary<(Type Type, string? Profile), object> bindings;
    public ByteMapperBinding<T>? GetBinding<T>(string? profile = null)
        => bindings.TryGetValue((typeof(T), profile), out var v) ? (ByteMapperBinding<T>)v : null;
}
```

MVC では `[ByteMapperProfile("v1")]` 属性で `HttpContext.Items` にキー文字列を入れて Formatter 側がそれを使って `GetBinding<T>(profile)` を呼ぶ流れに変更する。Minimal API では `.WithByteMapperBody<OrderRecord>(profile: "v1")` のような拡張で渡す。

---

## 8. 配列・コレクション対応

### 8.1 旧版の問題
- `typeof(ArrayInputReader<>).MakeGenericType(elem)` で要素型ごとの reader を実行時に組み立てていた。AOT 非適合。

### 8.2 新版の方針

| 対象型 | 対応 |
| --- | --- |
| `T[]`(単一要素型 T) | `ByteMapperArrayBinding<T>` を **Generator が事前生成**。Formatter/Filter は配列判定 + 要素 Binding ルックアップで処理。 |
| `List<T>` / `IList<T>` / `IEnumerable<T>` | 同じく `ByteMapperArrayBinding<T>` を使用。コレクション種別の判定は AspNetCore 側で `IsAssignableFrom` レベルで行う(`MakeGenericType` は不要)。 |
| 多次元配列 / 入れ子コレクション | サポートしない(Diagnostic 警告)。 |

### 8.3 ストリーミング Read/Write
- 旧版同様 `ArrayPool<byte>.Shared` を使い、`mapper.Size` 単位で読み書きする。
- Generator 側コード生成では特に変化なし。AspNetCore ランタイムが `ByteMapperArrayBinding<T>.ElementSize` を参照してループ展開する。

---

## 9. Generator が emit するコード(完全イメージ)

```csharp
// 1) ユーザコード
[ByteMapperEndpoint]
internal static partial class OrderMappers
{
    [ByteReader] public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);
    [ByteWriter] public static partial void Write(OrderRecord source, Span<byte> buffer);
}

// 2) ByteMapperGenerator が emit する OrderMappers.g.cs (本体)
partial class OrderMappers
{
    private static readonly global::Smart.IO.ByteMapper.Converters.BinaryConverter<int> Converter0_0 = new(global::Smart.IO.ByteMapper.Endian.Big);
    private static readonly global::Smart.IO.ByteMapper.Converters.TextConverter Converter0_1 = new(32, 20127, true, global::Smart.IO.ByteMapper.Padding.Right, (byte)0x20);
    // … Read/Write 実装 …
}

// 3) ByteMapperAspNetCoreGenerator が emit する OrderMappers.AspNetCore.g.cs (新規)
partial class OrderMappers
{
    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding<global::MyApp.OrderRecord> CreateByteMapperBinding()
        => new(
            size: 40,
            read:  static (s, t) => Read(s, t),
            write: static (s, d) => Write(s, d),
            factory: static () => new global::MyApp.OrderRecord());

    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperArrayBinding<global::MyApp.OrderRecord> CreateByteMapperArrayBinding()
        => new(
            elementSize: 40,
            readElement:  static (s, t) => Read(s, t),
            writeElement: static (s, d) => Write(s, d),
            factory:      static () => new global::MyApp.OrderRecord());
}

// 4) アセンブリ全体のレジストリ登録ヘルパ __ByteMapperAspNetCoreBootstrap.g.cs
namespace Smart.IO.ByteMapper.AspNetCore.Generated;
public static class __Bootstrap
{
    public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry Build()
    {
        var single = new global::System.Collections.Generic.Dictionary<(global::System.Type, string?), object>
        {
            { (typeof(global::MyApp.OrderRecord), null), global::MyApp.OrderMappers.CreateByteMapperBinding() },
        };
        var array = new global::System.Collections.Generic.Dictionary<(global::System.Type, string?), object>
        {
            { (typeof(global::MyApp.OrderRecord), null), global::MyApp.OrderMappers.CreateByteMapperArrayBinding() },
        };
        return new global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry(single, array);
    }
}
```

DI 登録拡張(ランタイム側の手書きコード):

```csharp
public static class ByteMapperServiceCollectionExtensions
{
    public static IServiceCollection AddByteMapperFormatters(this IServiceCollection services, Action<ByteMapperFormatterOptions>? configure = null)
    {
        var options = new ByteMapperFormatterOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddSingleton(global::Smart.IO.ByteMapper.AspNetCore.Generated.__Bootstrap.Build());
        services.AddSingleton<ByteMapperInputFormatter>();
        services.AddSingleton<ByteMapperOutputFormatter>();
        return services;
    }
}
```

> ランタイム側コードから Generator 出力の `__Bootstrap` 型を参照する形になる。これは **ユーザコードがコンパイルされた後にしか存在しない型** を参照することになるため、`Smart.IO.ByteMapper.AspNetCore` を「ユーザコード側の partial assembly」とみなす、もしくは **Generator が `AddByteMapperFormatters` 拡張も含めて emit する** 形に変更する必要がある。後者を採用する案を §10 で検討する。

---

## 10. Generator パッケージ構成の検討

### 10.1 課題
`AddByteMapperFormatters` の中で Generator 出力型を参照するためには、ランタイムアセンブリ `Smart.IO.ByteMapper.AspNetCore` ではなく **ユーザのアセンブリ** に拡張メソッドを置く必要がある。さもなければ循環参照になる。

### 10.2 案

| 案 | 概要 | トレードオフ |
| --- | --- | --- |
| **A. Generator がユーザアセンブリに `AddByteMapperFormatters` を生成** | ユーザ assembly に `MyApp.ByteMapperServiceCollectionExtensions.AddByteMapperFormatters(this IServiceCollection)` を生成。名前空間はユーザの root namespace。 | ユーザ毎に名前空間が変わるが UX 上問題なし(`global using` で吸収可能)。AOT 完全適合。 |
| B. ランタイムが「Bootstrapper 取得用デリゲート」を受け取る | `services.AddByteMapperFormatters(() => __Bootstrap.Build())` のように呼び出し側でファクトリ渡し | ユーザの 1 行が増えるが構造はシンプル |
| C. Generator も AspNetCore も同一アセンブリに同居 | 旧版同様、ByteMapper 本体と AspNetCore を 1 つの DLL に統合 | デプロイサイズ増。AspNetCore を使わない場合の依存問題。 |

**推奨: A 案**(Generator がユーザアセンブリに拡張メソッドを生成)。Smart.Mapper.Generator にも似たパターンがあるかは要確認だが、AOT 適合の点で最もクリーン。

### 10.3 Generator 構成
- `Smart.IO.ByteMapper.Generator` 本体に AspNetCore 機能を **オプショナル分離** する。`[ByteMapperEndpoint]` 属性が含まれるシンボルが Compilation に存在するときのみ AspNetCore 関連コード生成パスを起動する。
- 「AspNetCore を参照しないコンソールアプリ」では `[ByteMapperEndpoint]` 属性自体が解決できず、何も生成されないため副作用ゼロ。

---

## 11. 移行ガイド(旧版ユーザ向け)

| 旧 | 新 |
| --- | --- |
| `new ByteMapperFormatterConfig { MapperFactory = factory, ... }` | `builder.Services.AddByteMapperFormatters(options => { ... })`(`MapperFactory` 不要) |
| `mvc.InputFormatters.Add(new ByteMapperInputFormatter(config))` | `mvc.InputFormatters.Insert(0, sp.GetRequiredService<ByteMapperInputFormatter>())` |
| `[ByteMapperProfile("v1")]` | 同名属性をそのまま提供。ただし内部実装はプロファイルキーで Generator 生成 Binding を引く方式 |
| `.WithByteMapperFilter(config)` | `.WithByteMapperBody<OrderRecord>()`(強型版)/ `.WithByteMapperFilter()`(弱型互換版) |
| `MapperFactory.Create<T>()` | 不要。Generator が partial method を生成し、Binding は自動登録される |

---

## 12. 性能・AOT への影響評価

### 12.1 性能
- Single エンティティの Read/Write は **デリゲート 1 回呼び出し + キャスト 1 回**。旧版の `MapperFactory.Create()` 経由 → `ITypeMapper<T>` インタフェース呼び出し → boxing の経路に比べ、確実に高速。
- 参照型については boxing 完全消滅。値型レコードのみ ObjectFormatter 経路で boxing が 1 回発生する可能性があり、ホット用途では §5.3 B 案(専用 Formatter)を選ぶ余地を残す。

### 12.2 AOT
- `MakeGenericType` / `Activator.CreateInstance` / `Type.GetConstructor` を一切使用しない設計。
- `typeof(T)` ベースのレジストリ参照は **コンパイル時に解決された閉じた型** のみを使用するため、Trimmer/AOT で削除されない。
- ランタイム例外時のエラーメッセージで `typeof(T).FullName` を含めることは可能だが、`Type.Name` 取得自体は AOT 適合。

---

## 13. 残課題・要決定事項

1. **`ByteMapperBinding` を抽象基底にするか / 完全ジェネリックに留めるか**: §5.3 の A/B 選択。基本は A(抽象基底)で進めるが、値型レコードのホットパス向けに B(専用 Formatter)を後追いで追加する案。
2. **配列ストリーミング Read の Content-Length 不明時の戦略**: 旧版は `List<T>.ToArray()` で確定。`net8.0+` の `IBufferWriter<byte>` を使えるかは要検証。
3. **`HttpContext.Items` 経由のプロファイル受け渡し**: 文字列キーで良いか、`enum` ベースの型安全化を提供すべきか。当面は文字列(互換性優先)。
4. **OpenAPI / Swagger 連携**: 旧版同様 Schema 生成は対象外とするか、`Microsoft.AspNetCore.OpenApi` の `OperationFilter` で MediaType を案内する補助だけ提供するか。
5. **DI スコープ**: `ByteMapperRegistry` / Formatters は Singleton で安全(全ステートが起動時に確定)。
6. **`MapperFactory` の旧 API 互換**: 旧 API を残すかどうか。残さない場合、旧版ユーザの自動マイグレーションをどう支援するか(Analyzer での誘導 Diagnostic)。
7. **Generator アセンブリ参照**: §10 で A 案を採る場合、`Smart.IO.ByteMapper.Generator` に AspNetCore 機能を統合するか別 `Smart.IO.ByteMapper.AspNetCore.Generator` を立てるか。後者の方がオプショナルにしやすい。

---

## 14. まとめ

- 旧 `Smart.IO.ByteMapper.AspNetCore` の **リフレクション依存(`MakeGenericType` / `Invoke` / `MapperFactory.Create(Type)`)を完全に排除** し、Source Generator が事前生成した `ByteMapperBinding<T>` を `FrozenDictionary` ベースのレジストリで解決する設計に置き換える。
- ユーザ宣言は `[ByteMapperEndpoint]` 属性を `static partial class` に付けるだけ。Generator が **Binding 提供メソッド + アセンブリ全体のレジストリ初期化コード + `AddByteMapperFormatters` 拡張メソッド** を自動生成する。
- Minimal API では `.WithByteMapperBody<T>()` の強型版を主とし、旧弱型版は互換用に残す。
- Profile は文字列キーではなく **Profile 用 partial class を別途宣言する** スタイルへ移行。レジストリは `(Type, ProfileKey)` の複合キー対応。
- AOT/Trim 完全適合・boxing 撲滅(参照型完全 / 値型は §5.3 で選択肢提供)・既存 MVC/Minimal API パターンとの整合性を保ったまま再構築できる。
