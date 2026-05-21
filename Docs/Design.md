# Smart.IO.ByteMapper (Source Generator 版) 設計指示書

最終更新: 2026-05-21

本書は、AOT 対応を目的として既存の `Smart.IO.ByteMapper`(リフレクション + 動的型生成版) を Source Generator ベースで再構築する際の **実装指示書** である。本書に列挙する各要件 (「〜すること」「〜とする」) は実装の必須要件であり、逸脱する場合は事前に本書を改訂すること。Source Generator の構成・パイプラインは `__Reference/Mapper`(Smart.Mapper.Generator) に倣い、固定長レコードのマッピング仕様は `__Reference/ByteMapper`(Smart.IO.ByteMapper) を踏襲すること。

---

## 1. 目的・スコープ・前提

### 1.1 目的(必達事項)
- ランタイムのリフレクション / Reflection.Emit / Expression Tree への依存を **完全に排除** し、Native AOT・Trim に適合させること。
- 既存ライブラリ相当の固定長バイト ↔ POCO 変換機能を、**コンパイル時生成コード** で提供すること。
- 属性体系と意味論は既存版との **互換性を保つこと**(既存ユーザのコード移行を容易にする)。
- 性能(スループット・GC ゼロアロケーション)を最優先目標とし、生成コードは **手書き相当の効率** に近づけること。

### 1.2 非ゴール
- ストリーミング型の可変長フォーマット(JSON / XML 等)対応は本フェーズの対象外とする。
- 既存 `Smart.IO.ByteMapper.AspNetCore` / `Smart.IO.ByteMapper.Options` 等のサテライト機能は本フェーズでは対象外とする(後追い検討)。

### 1.3 参照実装(必ず参照すること)
| 役割 | 参照プロジェクト |
| --- | --- |
| Source Generator の組み方 | `__Reference/Mapper/Smart.Mapper.Generator` |
| 属性体系・固定長レコード仕様 | `__Reference/ByteMapper/Smart.IO.ByteMapper` |
| AOT テストの設定 | `__Reference/Mapper/Smart.Mapper.AotTests` |

---

## 2. プロジェクト構成と全体方針

### 2.1 構成プロジェクト(必須構成)
以下の構成で実装すること。既存リポジトリの雛形をそのまま採用する。

| プロジェクト | TFM | 役割 |
| --- | --- | --- |
| `Smart.IO.ByteMapper` | `net8.0;net9.0;net10.0` | 属性 / Enum / 例外 / 拡張用基底など、ユーザ・Generator 双方が参照するランタイムアセンブリ。コードジェネレータの DLL は `analyzers/dotnet/cs` にパッケージされる。 |
| `Smart.IO.ByteMapper.Generator` | `netstandard2.0` (IIncrementalGenerator) | 属性付き partial メソッドを解析しコード生成する Roslyn Source Generator。`SourceGenerateHelper` を利用。 |
| `Smart.IO.ByteMapper.Tests` | `net10.0` | xUnit による単体テスト。生成コードの結果検証も含む。 |
| `Develop` | `net10.0` | 動作確認用ホスト。 |
| `Smart.IO.ByteMapper.AotTests`(新規) | `net10.0`, `PublishAot=true` | AOT 互換性確認用。Smart.Mapper.AotTests と同等。 |
| `Smart.IO.ByteMapper.Benchmark`(新規) | `net10.0` | BenchmarkDotNet による性能計測。 |

### 2.2 配布
- NuGet パッケージは `Smart.IO.ByteMapper` を主とすること。
- `PackBuildOutputs` ターゲットで Generator DLL と SourceGenerateHelper を analyzer として同梱すること(既存 `.csproj` に組み込み済み)。

### 2.3 利用フロー(参考図)
```
ユーザコード (partial method 宣言 + 属性)
       │
       ▼
Generator (Roslyn 解析 → モデル化 → 生成)
       │
       ▼
.g.cs (partial 実装) … BinaryPrimitives / Span 操作で直接 read/write
       │
       ▼
コンパイル → 実行(AOT 可)
```

### 2.4 主要要素一覧(実装する型の概要)
- **ユーザ側属性(Generator 側で読む):**
  - メソッド属性: `[ByteReader]` / `[ByteWriter]`
  - 型属性: `[Map]`, `[MapFiller]`, `[MapConstant]`
  - メンバ属性: `[MapBinary<T>]`, `[MapText]`, `[MapByte]`, `[MapBytes]`, `[MapBoolean]`, `[MapNumberText<T>]`, `[MapDateTimeText<T>]`, `[MapArray<TElement, TElementConverter>]`(いずれも `ByteMapperConverterAttribute<TConverter>` 派生として実装すること)
  - 共通 enum: `Endian`, `Padding`, `Culture`
  - 拡張用基底: `ByteMapperPropertyAttribute`(抽象クラス) / `ByteMapperConverterAttribute<TConverter>`(ジェネリック基底)
- **ランタイム共通型(Converter 群):**
  - インスタンス Converter クラス: `BinaryConverter<T>`, `TextConverter`, `BooleanConverter`, `BytesConverter`, `NumberTextConverter<T>`, `DateTimeTextConverter<T>` ── 旧 `IMapConverter` を引き継ぎ、コンストラクタで設定値を保持する **イミュータブル sealed class** として再構築すること。`object` 経由の boxing を排除した generic `Read`/`Write` を提供すること。
  - 配列は専用 Converter を設けず、**Generator が `for` ループを直接 emit** し、要素 Converter のインスタンスメソッドを呼ぶこと。
  - `ByteMapperException`
  - `Helpers/BytesHelper` 相当(`Fill`, `TrimRange`, `CopyBytes` 等)を `Smart.IO.ByteMapper.Internal` で公開し、Converter 群および生成コードから呼び出すこと。
- **Generator 構成要素:** Models / Helpers / Diagnostics / `ByteMapperGenerator`(IIncrementalGenerator)

---

## 3. ユーザ API: 生成対象メソッドのシグネチャ

### 3.1 基本方針(必須要件)
- ユーザに `static partial` メソッドを宣言させ、`[ByteReader]` または `[ByteWriter]` を付与する形を採ること。
- Generator は同名 partial 実装を `.g.cs` に生成すること(Smart.Mapper.Generator と同じ流儀)。
- メソッドが属するクラス/構造体も `partial` であることを要求すること。

### 3.2 サポートするシグネチャ(必須実装)

以下 4 種のシグネチャを必須実装とすること。

| 種別 | 属性 | シグネチャ |
| --- | --- | --- |
| Read (in-place) | `[ByteReader]` | `static partial void <Name>(ReadOnlySpan<byte> buffer, T target)` |
| Read (new instance) | `[ByteReader]` | `static partial T <Name>(ReadOnlySpan<byte> buffer)` |
| Write (caller buffer) | `[ByteWriter]` | `static partial void <Name>(T source, Span<byte> buffer)` |
| Write (allocate) | `[ByteWriter]` | `static partial byte[] <Name>(T source)` |

オプション拡張(設計上許容、初期フェーズでは上記 4 種を必須):
- `Stream` 入出力版(オーバーロード)。ストリーミング機能は拡張メソッドとして別途提供してもよい。
- `int offset` を受け取って Span をスライスせず使う版。

### 3.3 制約(検証して Diagnostic 化すること)
- メソッドは `static partial` 必須とする。アクセシビリティ(public/internal/private/file)は自由とする。
- 属性の付いていない部分は通常の partial メソッドと同じ規約に従う(同一クラスに実装が無いと CS8795)。生成側はバインダで補うこと。
- ジェネリック型引数は **不可** とする(後段の `T` は具象型である必要がある)。Smart.Mapper 同様、メソッド単位で型を確定させること。
- 同一メソッド名で複数の `T` を扱いたい場合は別メソッド名のオーバーロードを宣言させること。

### 3.4 ユーザコード例(参考)

```csharp
internal static partial class RecordMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> buffer, OrderRecord target);

    [ByteReader]
    public static partial OrderRecord Read(ReadOnlySpan<byte> buffer);

    [ByteWriter]
    public static partial void Write(OrderRecord source, Span<byte> buffer);

    [ByteWriter]
    public static partial byte[] Write(OrderRecord source);
}
```

---

## 4. 属性定義

### 4.1 メソッド属性(必須実装)

以下の属性を実装すること。

```csharp
[AttributeUsage(AttributeTargets.Method)]
public sealed class ByteReaderAttribute : Attribute
{
    public Type? Profile { get; set; }       // 既存クラスに属性を付けられない場合の代替 (§5)
    public bool ValidateLayout { get; set; } = true;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ByteWriterAttribute : Attribute
{
    public Type? Profile { get; set; }
    public bool ValidateLayout { get; set; } = true;
}
```

挙動要件:
- `Profile` 未指定時はメソッド第 1(または戻り値) 引数の Target 型に直接付いた属性を使ってマッピングを行うこと。
- `ValidateLayout` が `true` のときオフセット重複や Size 超過を Diagnostic として報告すること(生成は続行する)。
- `[ByteReader]` と `[ByteWriter]` の分割は必須とする(意図明示・拡張余地確保のため)。

### 4.2 型属性 (`AttributeTargets.Class | Struct`)(必須実装)

| 属性 | 役割 | コンストラクタ | 主なプロパティ |
| --- | --- | --- | --- |
| `MapAttribute` | 固定長レコード長と全体既定 | `Map(int size)` | `bool AutoFiller=true`, `bool UseDelimiter=true`, `byte? NullFiller`, `byte[]? Delimiter` |
| `MapFillerAttribute` | 詰め物領域 | `MapFiller(int offset, int length)` | `byte Filler` |
| `MapConstantAttribute` | 固定値領域 | `MapConstant(int offset, byte[] content)` | (なし) |

互換性要件:
- 既存版では `MapAttribute` の `Delimiter` は `Parameter.Delimiter` 経由でグローバル設定だったが、Source Generator では実行時パラメータ参照を持たないため、**型属性で直接指定する形に統合すること**(未指定なら `CRLF` を既定値とする)。

### 4.3 メンバ属性 (`AttributeTargets.Property`)(必須実装)

組み込み属性は **すべて `ByteMapperConverterAttribute<TConverter>` を継承する**(§6)。ユーザ拡張属性と同じ仕組みに乗らせること。Generator は基底のジェネリック型引数から Converter クラスを取得し、属性のコンストラクタ引数 / 名前付きプロパティを Converter のコンストラクタに渡してインスタンスを構築する。

| 属性 | 継承先 (TConverter) | コンストラクタ | プロパティ | 対象 .NET 型 |
| --- | --- | --- | --- | --- |
| `MapBinaryAttribute<T>` | `BinaryConverter<T>` | `(int offset)` | `Endian Endian` | `T : unmanaged` (+ `Nullable<T>`) |
| `MapByteAttribute` | `ByteConverter` | `(int offset)` | — | `byte` |
| `MapBytesAttribute` | `BytesConverter` | `(int offset, int length)` | `byte Filler` | `byte[]` |
| `MapTextAttribute` | `TextConverter` | `(int offset, int length)` | `int CodePage`, `bool Trim`, `Padding Padding`, `byte Filler` | `string` |
| `MapBooleanAttribute` | `BooleanConverter` | `(int offset)` | `byte TrueValue`, `byte FalseValue`, `byte NullValue` | `bool`, `bool?` |
| `MapNumberTextAttribute<T>` | `NumberTextConverter<T>` | `(int offset, int length)` | `string? Format`, `int CodePage`, `bool Trim`, `Padding Padding`, `byte Filler`, `NumberStyles Style`, `Culture Culture` | 数値型, Nullable |
| `MapDateTimeTextAttribute<T>` | `DateTimeTextConverter<T>` | `(int offset, int length, string format)` | `int CodePage`, `byte Filler`, `DateTimeStyles Style`, `Culture Culture` | `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, Nullable |
| `MapArrayAttribute<TElementAttribute>` | (要素属性 `TElementAttribute` の Converter を再利用) | `(int offset, int count)` | `byte Filler` | `TElement[]` ── 詳細は §6.10 参照 |

#### 4.3.1 共通基底(拡張可能性を担保するため必須実装)

```csharp
[AttributeUsage(AttributeTargets.Property)]
public abstract class ByteMapperPropertyAttribute : Attribute
{
    protected ByteMapperPropertyAttribute(int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        Offset = offset;
    }
    public int Offset { get; }
}
```

実装規約:
- 組み込み・拡張のいずれも `ByteMapperPropertyAttribute` → `ByteMapperConverterAttribute<TConverter>` の派生として実装すること(§6)。
- Generator は **「対象属性かどうか」を `AttributeClass` の基底チェーンで判定** すること(`ByteMapperPropertyAttribute` 派生かどうか)。
- Converter の結び付けは、属性が `ByteMapperConverterAttribute<TConverter>` の派生であれば、その閉じた型引数 `TConverter` から **自動取得すること**。組み込み属性も同じ仕組みで処理し、Generator 側に固有の分岐を **持たせないこと**(Diagnostic 用の型検証を除く)。

### 4.4 enum 定義(必須実装)

| Enum | 値 |
| --- | --- |
| `Endian` | `Big, Little`(規定 `Big`) |
| `Padding` | `Left, Right`(テキスト系既定 `Right`、数値テキスト既定 `Left`) |
| `Culture` | `Invariant, Current`(規定 `Invariant`) |

要件: `Culture.Current` を選択した場合、生成コードは `CultureInfo.CurrentCulture` を直接参照すること。

### 4.5 既定値 (`Parameter` 静的クラス相当)

実装方針:
- ランタイムでパラメータコンテナを引き回す代わりに、Generator 内部に固定既定値を持たせること。
- ユーザはアセンブリ属性 `[assembly: ByteMapperDefaults(...)]` で既定を上書きできるようにすること(任意拡張)。

```csharp
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class ByteMapperDefaultsAttribute : Attribute
{
    public string EncodingName { get; set; } = "us-ascii";
    public byte Filler { get; set; } = 0x20;
    public byte TextFiller { get; set; } = 0x20;
    public Padding TextPadding { get; set; } = Padding.Right;
    public Endian Endian { get; set; } = Endian.Big;
    public byte TrueValue { get; set; } = 0x31; // '1'
    public byte FalseValue { get; set; } = 0x30; // '0'
    public byte[]? Delimiter { get; set; }       // 既定 CRLF
    public bool Trim { get; set; } = true;
}
```

要件: 属性のプロパティに「未指定」を表現するため、enum/値型はオプショナル時のみ `int?` ベースのナラブル相当を使うか、`bool ExplicitlySet` フラグを Smart.Mapper のように追加すること(Generator 側で Symbol API から判別すること)。

---

## 5. プロファイル方式(既存クラスに属性を付けられない場合)

### 5.1 適用条件
以下のいずれかに該当する場合、本機能を適用すること。
- サードパーティ製クラスなど、ソースを編集できない型を Target にしたい場合。
- 同一型に対して複数のレイアウト(プロトコル A/B)を切り替えたい場合。

### 5.2 仕様(実装規約)
1. ユーザに Target と同じ「プロパティ名・型」を持つ **Profile クラス** を作成させ、そこに `[Map]`/`[MapXxx]` 属性を付けさせること。
2. `[ByteReader]`/`[ByteWriter]` の `Profile` プロパティに Profile 型を渡させること。
3. Generator は Target 型の属性ではなく Profile 型のメタデータを参照してマッピングを生成すること(プロパティのアクセサ呼び出しは Target 型に対して行うこと)。

利用例(参考):

```csharp
// 編集できない既存型
public sealed class LegacyOrder
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ユーザが書く Profile
[Map(40)]
internal sealed class LegacyOrderProfile
{
    [MapBinary(0)] public int Id { get; set; }
    [MapText(4, 32)] public string Name { get; set; } = string.Empty;
}

internal static partial class LegacyMappers
{
    [ByteReader(Profile = typeof(LegacyOrderProfile))]
    public static partial void Read(ReadOnlySpan<byte> buffer, LegacyOrder target);
}
```

### 5.3 解決ルール(必須要件)
- Profile 型のプロパティ名と Target 型のプロパティ名を **大小区別ありで一致** させること(現時点では Mapper のように `StringComparison` 切替は予定しない)。
- Target に同名プロパティが無い場合 → Diagnostic `SBM0011` を発行すること。
- Target プロパティの型と Profile 属性の許容型が合わない場合 → Diagnostic `SBM0012` を発行すること。
- Profile クラスに付いた `[Map]`/`[MapConstant]`/`[MapFiller]` は型属性として採用すること。
- Profile 自体は POCO で良く、`abstract`/`sealed` どちらでも構わない。Generator は型を参照するだけで、**インスタンス化しないこと**。

> 設計選択肢比較は付録 A.1 を参照。

---

## 6. Converter 契約と属性体系(組み込み・拡張で共通)

### 6.1 基本原則(必須要件)
- 旧 `Smart.IO.ByteMapper` の `IMapConverter`(`object` 経由・boxing 前提) を **インスタンス契約 + generic Read/Write** に進化させること(boxing と reflection を排除した自然な後継)。
- 新版では **「Converter = `sealed class`(イミュータブル)」「属性 = `ByteMapperConverterAttribute<TConverter>` の派生」** という二要素のみで成立する **単一プロトコル** を採用すること。
- Converter のインスタンスは **Mapper 側 `partial class` に Generator が emit する `private static readonly` フィールドとして 1 回だけ構築** すること。設定値(Encoding / CultureInfo / フォーマット文字列など)はコンストラクタで前解決させ、毎回の `Read`/`Write` には伴わせないこと。
- 組み込み属性 (`MapBinary`, `MapText`, …) と、ユーザが追加する派生属性 (`MapHex` 等) は **完全に同じ仕組み** に乗せること。Generator は組み込みかどうかを区別せず、`TConverter` を辿ってインスタンスを構築 / メソッドを呼び出すこと。
- 属性自体はランタイムで `new` しないこと(コンパイル時データホルダーとする)。Generator は属性宣言の `ConstructorArguments` / `NamedArguments` を読むのみ。

### 6.2 基底クラス階層(必須実装)

```csharp
namespace Smart.IO.ByteMapper;

// すべてのプロパティ用属性の最終的な基底
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class ByteMapperPropertyAttribute : Attribute
{
    protected ByteMapperPropertyAttribute(int offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        Offset = offset;
    }
    public int Offset { get; }
}

// 「型引数 TConverter の静的 Converter を使う属性」を表す抽象基底
// 組み込み属性も拡張属性も、すべてこれを継承する
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class ByteMapperConverterAttribute<TConverter> : ByteMapperPropertyAttribute
{
    protected ByteMapperConverterAttribute(int offset) : base(offset) { }
}
```

要件: ジェネリック属性は C# 11 / .NET 7+ で利用可能。本プロジェクトは `net8.0` 以上を対象とするため使用を許可する。

### 6.3 Converter インスタンス契約(必須要件)

Converter は **`sealed class`(イミュータブル)** とし、以下のメンバ構成を必ず持たせること。Generator は Converter のインスタンスを **Mapper 側に `private static readonly` フィールドで保持し**、`Read`/`Write` のたびにそのフィールドを経由して呼び出す。

```csharp
// 概念上の契約(言語的なインタフェースは置かない ── 戻り値型がプロパティ型に応じて変わるため)
public sealed class TConverter
{
    // (A) コンストラクタ ── 属性から取り出した設定値を受け取り、内部で前解決を済ませる
    public TConverter(/* attr args */) { /* Encoding.GetEncoding(codePage) などをここで一度だけ */ }

    // (B) サイズ
    public int Size { get; }                    // (B-1) コンストラクタで確定する固定サイズ
    // または
    public const int Size = N;                  // (B-2) すべてのインスタンスで一定の固定サイズ(BinaryConverter<T> など)

    // (C) 読み込み(インスタンスメソッド・追加引数なし)
    public TProperty Read(ReadOnlySpan<byte> source);

    // (D) 書き込み(インスタンスメソッド・追加引数なし)
    public void Write(Span<byte> destination, TProperty value);
}
```

実装規約(全 Converter で必須):
1. Converter は **`sealed class`** とすること。フィールドはすべて `readonly` でイミュータブルにすること。スレッド安全に並列利用される前提とすること。
2. **コンストラクタの引数並びは、属性側の「コンストラクタ引数(`offset` を除く)→ 名前付きプロパティ」の宣言順にそのまま対応させること。**
   - 例: `[MapText(0, 32, CodePage = 932, Trim = false)]` → `new TextConverter(length: 32, codePage: 932, trim: false, padding: <default>, filler: <default>)`
   - 属性側で未指定の名前付きプロパティは、属性が宣言する **既定値リテラル** を Generator がそのまま埋め込むこと(`default(T)` ではなく属性の既定値)。
3. **サイズ** は `int Size { get; }`(コンストラクタで確定するインスタンスプロパティ)または `const int Size`(全インスタンス共通の固定サイズ)のいずれかを持たせること。両方を持つ場合は `const Size` を優先すること(`BinaryConverter<T>` のように `Unsafe.SizeOf<T>()` で確定するケース向け)。
4. **`Read`** の戻り値はプロパティ型と一致させること(`Nullable<T>` のラップ含む)。引数は `ReadOnlySpan<byte> source` の 1 つのみとすること。
5. **`Write`** の引数は `Span<byte> destination`, `TProperty value` の 2 つのみとすること。
6. `Read`/`Write` に渡される `source`/`destination` は **既に `buffer.Slice(offset, size)` で対象領域に切り出された Span** であることを前提とすること。Converter 側で `offset` を意識しないこと。
7. **null 対応**: プロパティ型が `Nullable<T>` または参照型 nullable な場合、Generator は Converter を呼ぶ前後に null チェックを挟むこと。
   - Read: Converter の戻り値をそのまま代入(Converter は `Nullable<T>` を返してよい)。
   - Write: `value` が `null` のとき Converter を呼ばずに `destination.Fill(filler)` のみとすること。

### 6.4 組み込み Converter(必須実装一覧)

ランタイム (`Smart.IO.ByteMapper.Converters`) に以下の `sealed class` を実装すること。**いずれも §6.3 の契約に従わせること**。

| Converter | コンストラクタ | 主なフィールド(前解決済み) | `Size` 種別 |
| --- | --- | --- | --- |
| `BinaryConverter<T> where T : unmanaged` | `(Endian endian)` | `Endian` | `const Size = Unsafe.SizeOf<T>()` 相当(`static readonly`) |
| `ByteConverter` | `()` | —(シングルトン共有可) | `const Size = 1` |
| `BytesConverter` | `(int length, byte filler)` | `int length, byte filler` | インスタンス `Size = length` |
| `TextConverter` | `(int length, int codePage, bool trim, Padding padding, byte filler)` | `int length, Encoding encoding, bool trim, Padding padding, byte filler` | インスタンス `Size = length` |
| `BooleanConverter` | `(byte trueValue, byte falseValue, byte nullValue)` | `byte trueValue, falseValue, nullValue` | `const Size = 1` |
| `NumberTextConverter<T>` | `(int length, string? format, int codePage, bool trim, Padding padding, byte filler, NumberStyles style, Culture culture)` | `int length, string? format, Encoding encoding, bool trim, Padding padding, byte filler, NumberStyles style, IFormatProvider provider` | インスタンス `Size = length` |
| `DateTimeTextConverter<T>` | `(int length, string format, int codePage, DateTimeStyles style, Culture culture, byte filler)` | `int length, string format, Encoding encoding, DateTimeStyles style, IFormatProvider provider, byte filler` | インスタンス `Size = length` |

実装ポイント(必達):
- **`Encoding` / `IFormatProvider` をコンストラクタで前解決すること**: `Encoding.GetEncoding(codePage)` や `Culture == Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture` を一度だけ評価し、フィールドに保持すること。`Read` / `Write` の毎呼び出しで再評価しないこと。
- `BinaryConverter<T>` 内部では `typeof(T) == typeof(int)` 形式の分岐で `BinaryPrimitives.ReadInt32BigEndian` 等を呼び出すこと。JIT / AOT のジェネリック特殊化により未使用分岐は除去される。
- `ByteConverter` は引数なしのため、ランタイム内に `public static readonly ByteConverter Instance = new();` を置いてシングルトン共有可能とすること。Generator もそのフィールドを参照すれば良い(オプション)。
- これらの Converter は **生成コードから直接呼ばれる** ため、ユーザの感覚としてはランタイム DLL 同梱の通常クラスとして公開すること。
- 配列(`MapArray`)は専用 Converter を設けないこと。詳細は §6.10。

### 6.5 組み込み属性の実装(必須実装)

組み込み属性は **すべて `ByteMapperConverterAttribute<TConverter>` を継承させること**。Generator にとってはユーザ拡張属性と同じパスで処理させる。属性の値は Generator が読み取り、対応する Converter のコンストラクタ呼び出しコードに変換する。

```csharp
// 例: MapBinaryAttribute → new BinaryConverter<int>(Endian.Big)
public sealed class MapBinaryAttribute<T> : ByteMapperConverterAttribute<BinaryConverter<T>>
    where T : unmanaged
{
    public MapBinaryAttribute(int offset) : base(offset) { }
    public Endian Endian { get; init; } = Endian.Big;
}

// 例: MapTextAttribute → new TextConverter(length, codePage, trim, padding, filler)
public sealed class MapTextAttribute : ByteMapperConverterAttribute<TextConverter>
{
    public MapTextAttribute(int offset, int length) : base(offset)
    {
        Length = length;
    }
    public int Length { get; }
    public int CodePage { get; init; } = 20127;       // us-ascii
    public bool Trim { get; init; } = true;
    public Padding Padding { get; init; } = Padding.Right;
    public byte Filler { get; init; } = 0x20;
}

// 例: MapBooleanAttribute → new BooleanConverter(trueValue, falseValue, nullValue)
public sealed class MapBooleanAttribute : ByteMapperConverterAttribute<BooleanConverter>
{
    public MapBooleanAttribute(int offset) : base(offset) { }
    public byte TrueValue { get; init; } = 0x31;
    public byte FalseValue { get; init; } = 0x30;
    public byte NullValue { get; init; } = 0x20;
}
```

### 6.6 ユーザ拡張の最小実装パターン(指針)

ユーザは「Converter クラス」と「派生属性クラス」の 2 つを書くだけで拡張できる構造とすること。組み込み属性とまったく同じ書式で書けるようにする。

```csharp
// 1) Converter (sealed class) ── §6.3 の規約に従う
public sealed class MyHexConverter
{
    private readonly bool upperCase;

    public MyHexConverter(int length, bool upperCase)
    {
        Size = length;
        this.upperCase = upperCase;
    }

    public int Size { get; }

    public byte[] Read(ReadOnlySpan<byte> source)
    {
        /* hex デコード ── Size と upperCase はフィールドから参照 */
    }

    public void Write(Span<byte> destination, byte[] value)
    {
        /* hex エンコード */
    }
}

// 2) 属性 ── 基底のジェネリック型引数で Converter を結びつける
public sealed class MapHexAttribute : ByteMapperConverterAttribute<MyHexConverter>
{
    public MapHexAttribute(int offset, int length) : base(offset)
    {
        Length = length;
    }
    public int Length { get; }
    public bool UpperCase { get; init; }
}

// 3) 使用側 ── 組み込み属性と同じ感覚で書ける
public sealed class Packet
{
    [MapHex(0, 16, UpperCase = true)]
    public byte[] Token { get; set; } = default!;
}
```

Generator 挙動要件: `[MapHex(0, 16, UpperCase = true)]` が解析されると、Generator は `MapHexAttribute` の基底チェーンを辿って `ByteMapperConverterAttribute<MyHexConverter>` を発見し、Converter 型 = `MyHexConverter` を確定すること。属性のコンストラクタ引数 `length=16` + 名前付き `UpperCase=true` を宣言順に並べ替えて `new MyHexConverter(16, true)` を呼び、その結果を Mapper の `private static readonly` フィールドに保持すること。以後 Read/Write のたびに `<field>.Read(buffer.Slice(0, <field>.Size))` の形で呼び出させること。

### 6.7 Generator の解決手順(組み込み / 拡張で共通の必須実装)

1. プロパティから集めた属性のうち、`AttributeData.AttributeClass` の基底チェーンが `Smart.IO.ByteMapper.ByteMapperPropertyAttribute` に到達するものをマッピング対象とすること。
2. 同じく基底チェーン上に `Smart.IO.ByteMapper.ByteMapperConverterAttribute<>` の **構築型** (`ConstructedFrom` が一致するもの) を持つ場合、その閉じた型の `TypeArguments[0]` を **Converter Type** として取り出すこと。
3. Converter Type に対し、§6.3 の契約メンバを検索すること。
   - コンストラクタ: 公開コンストラクタ 1 つ。引数並びは属性のコンストラクタ引数(`offset` 除外) + 名前付きプロパティの宣言順と一致する必要がある。
   - サイズ: `int Size { get; }`(インスタンス) または `const int Size`(全インスタンス共通)。
   - 読み込み: `TProperty Read(ReadOnlySpan<byte> source)` ── 戻り値型 = プロパティ型(または `Nullable<T>` のラップを除いた型)。
   - 書き込み: `void Write(Span<byte> destination, TProperty value)`。
4. 属性のコンストラクタ引数 + 名前付きプロパティを宣言順に連結し、Converter のコンストラクタ引数並びと **一対一・型一致** で対応するかを検証すること。属性の名前付きプロパティで未指定のものは、属性側の **既定値リテラル** を Generator がコンストラクタ呼び出しコードに埋め込むこと。型 / 数 / 順序が合わない場合は Diagnostic `SBM0010` を発行すること。
5. **Converter インスタンス フィールドの命名(厳守)**: Mapper クラス(`partial class`)内に Generator が `private static readonly <ConverterTypeFqn> Converter<M>_<P> = new(<ctor args>);` を emit すること。
   - `<M>` = メソッドの連番(同一 `partial class` 内で `[ByteReader]/[ByteWriter]` を付けた `static partial` メソッドを Generator が走査した出現順)。
   - `<P>` = そのメソッド内のメンバマッピング連番(オフセット昇順)。
   - 例: `Converter0_3` = メソッド 0 のプロパティ 3 番目に対応する Converter。
   - Read/Write メソッド対(同じ Target/Layout)であっても、それぞれが独立した連番フィールド群を持つこと(共有しない)。これにより Generator のフィールド管理を単純化する(メモリは Converter 1 個分の重複のみで実用上無視できる)。
6. プロパティ型が `Nullable<T>` または参照型 nullable な場合は、生成コードが Read 後の null 取り扱い(Converter の戻り値そのまま代入) / Write 前の null チェック(null 時は filler 埋め)を挿入すること。
7. **Converter が組み込みかどうかは Generator のコード生成パスにとって無関係とすること**。組み込み属性は単に "Smart.IO.ByteMapper 名前空間にある `ByteMapperConverterAttribute<T>` 派生" として扱い、ユーザ拡張と同じパイプラインを通すこと。

> 設計選択肢比較は付録 A.2 を参照。

### 6.8 AOT・Trim 適合要件(必達)
- Converter のインスタンス化は **生成コード内で `new <ConverterTypeFqn>(args)` を直書き** すること。`Activator.CreateInstance` / `Type.GetConstructor` は **使用禁止**。
- Converter インスタンスは `private static readonly` フィールドに保持させ、Mapper クラスの静的コンストラクタ(または `field = new(...)` の inline init)で 1 回だけ構築させること。リフレクション経路は持たせないこと。
- `TConverter` はジェネリック型引数として属性の型情報そのものに表現されており、コード生成時に閉じた型として解決すること。生成コードは `new BinaryConverter<int>(Endian.Big)` / `new MyHexConverter(16, true)` の形で実型を直接参照するため、Trim 解析でも到達可能と判定され `[DynamicallyAccessedMembers]` は不要であること。
- 属性自体は実行時に new されないため、コンストラクタ本体は AOT で削除されてよい。Converter のコンストラクタとメソッド本体は、生成コードから静的に参照される限り保持される。

### 6.9 オープンジェネリック Converter の許可

プロパティ型に応じて Converter 実装を切替えたいケース(組み込みの `BinaryConverter<T>` がまさにこの構造)では、Converter 型を **オープンジェネリック** にできること。属性側もジェネリックにし、`ByteMapperConverterAttribute<MyConverter<T>>` に渡せばよい。

```csharp
public sealed class MyEndianConverter<T>
    where T : unmanaged
{
    private readonly Endian endian;
    public MyEndianConverter(Endian endian) { this.endian = endian; }

    public int Size => Unsafe.SizeOf<T>();
    public T Read(ReadOnlySpan<byte> source) { /* … */ }
    public void Write(Span<byte> destination, T value) { /* … */ }
}

public sealed class MapEndianAttribute<T> : ByteMapperConverterAttribute<MyEndianConverter<T>>
    where T : unmanaged
{
    public MapEndianAttribute(int offset) : base(offset) { }
    public Endian Endian { get; init; } = Endian.Big;
}

public sealed class Frame
{
    [MapEndian<int>(0, Endian = Endian.Little)] public int Length { get; set; }
    [MapEndian<long>(4)] public long Sequence { get; set; }
}
```

Generator 要件: 閉じたジェネリック構築型(`MyEndianConverter<int>`)を Converter Type として認識し、`new MyEndianConverter<int>(Endian.Little)` を `static readonly` フィールドで保持するコードを生成すること。組み込みの `MapBinaryAttribute<T> : ByteMapperConverterAttribute<BinaryConverter<T>>` と同じ構造として扱うこと。

### 6.10 配列 (`MapArray`) の取り扱い(必須要件)

固定長要素配列は **専用 Converter を持たせず**、Generator が `for` ループを直接 emit する形に統一すること。これにより:
- 要素 Converter は通常のインスタンス Converter とまったく同じ扱いとなる(別フィールドにも別途保持される)。
- 要素呼び出しは `sealed class` フィールド経由のインスタンスメソッド呼び出しで、JIT/AOT が非仮想として最適化する。
- 配列専用に追加の Converter ジェネリック構造を作らずに済む。

属性側は **要素属性をジェネリック型引数として受け取る** 形を採ること。要素属性は `ByteMapperConverterAttribute<TConverter>` 派生であれば何でも良い。

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MapArrayAttribute<TElementAttribute> : ByteMapperPropertyAttribute
    where TElementAttribute : ByteMapperPropertyAttribute, new()
{
    public MapArrayAttribute(int offset, int count) : base(offset)
    {
        Count = count;
    }
    public int Count { get; }
    public byte Filler { get; init; } = 0x00;

    // 要素属性のプロパティをそのまま MapArrayAttribute の名前付きプロパティとして公開する手段は無いため、
    // 別解 (案 X / 案 Y) を採用する ── §13 に詳述
}
```

> 配列はパラメータ受け渡しが複雑になりやすく(要素属性のコンフィグをどう持つか)、本書時点では **詳細仕様は §13 残課題** とすること。当面は要素プロパティ専用属性を別途用意する案を採る(例: `MapBinaryArrayAttribute<T>` / `MapTextArrayAttribute` 等の合成属性)。Generator は `[MapXxxArray]` を見つけたら、`new <ElementConverter>(...)` を `static readonly` フィールドで構築し、`for` ループで要素を読み書きするコードを生成すること。

生成コードイメージ(`[MapBinaryArray<int>(0, 10, Endian = Big)]` 相当):
```csharp
private static readonly global::Smart.IO.ByteMapper.Converters.BinaryConverter<int> Converter0_5
    = new(global::Smart.IO.ByteMapper.Endian.Big);

// Read 内:
var arr = new int[10];
for (var i = 0; i < 10; i++)
{
    arr[i] = Converter0_5.Read(buffer.Slice(20 + i * 4, 4));
}
target.Values = arr;
```

---

## 7. Source Generator 実装

### 7.1 形態(必須要件)
- `IIncrementalGenerator`(`[Generator]`)を使用すること。Roslyn 4.x。
- TFM: `netstandard2.0`、参照: `Microsoft.CodeAnalysis.CSharp 5.3.x`, `SourceGenerateHelper 1.7.x`(雛形と同じ)。

### 7.2 パイプライン(基準実装)

以下の構造を基準として実装すること。

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    var defaults = context.CompilationProvider.Select(GetAssemblyDefaults);

    var readers = context.SyntaxProvider
        .ForAttributeWithMetadataName("Smart.IO.ByteMapper.ByteReaderAttribute",
            static (s, _) => s is MethodDeclarationSyntax,
            static (ctx, _) => ParseMethod(ctx, MapperKind.Reader))
        .Collect();

    var writers = context.SyntaxProvider
        .ForAttributeWithMetadataName("Smart.IO.ByteMapper.ByteWriterAttribute",
            static (s, _) => s is MethodDeclarationSyntax,
            static (ctx, _) => ParseMethod(ctx, MapperKind.Writer))
        .Collect();

    var methods = readers.Combine(writers).Select(static (t, _) => t.Left.AddRange(t.Right));

    context.RegisterImplementationSourceOutput(
        defaults.Combine(methods),
        static (spc, src) => Execute(spc, src.Left, src.Right));
}
```

### 7.3 解析ステップ(必須実装)
1. **メソッド検証**: `static partial`、有効な 4 種シグネチャいずれかに該当することを検証すること。NG なら Diagnostic を `Result<T>` として返すこと。
2. **Target 型抽出**: `[ByteReader]/[ByteWriter]` で Target を引数または戻り値から決定すること。`Profile` が指定されていれば Profile 型を「属性ソース」として、Target 型を「コードで触れる型」として記録すること。
3. **型属性解析**: `MapAttribute` の `Size` を取得すること。`MapFiller` / `MapConstant` を列挙すること。
4. **メンバ属性解析**: Target/Profile の全プロパティを走査すること。`ByteMapperPropertyAttribute` 派生属性が付いていれば対象とすること。複数付与は最初の 1 件のみ採用(error)。`MapArray` は要素属性とのペアでのみ有効とすること。
5. **レイアウト解決**: 既存 `MapperPositionHelper.Layout` 相当の処理を Generator 内で実行すること。
   - オフセット昇順ソート
   - 範囲オーバーラップ検証(`ValidateLayout=true` 時)
   - ギャップに `MapFiller` 相当を自動挿入(`MapAttribute.AutoFiller=true` 時)
   - 末尾の Delimiter (CRLF) 自動付与(`UseDelimiter=true` 時)
6. **必須プロパティ検証**: アクセサ可否(get/set/init)、`record` 一次コンストラクタなど、`new T()` で生成できるかを判定すること。`T Read(...)` 版はオブジェクト構築可能性を必須化すること。
7. **生成可能エンコーディング検証**: `EncodingName`/`CodePage`/`Culture` の文字列値が既知のものか確認すること。`Encoding.GetEncoding(...)` 呼び出しの引数として埋め込むこと。
8. **Diagnostic 集約**: 解析中のエラーは `Result<T>` に格納し、出力時にまとめて報告すること。

### 7.4 内部モデル(必須実装)

組み込み・拡張の Converter をインスタンス契約 (§6.3) に統一したため、**個別の `ConverterModel` 派生は持たせず、単一の `ConverterCallModel`** で全パターンを表現すること。Mapper クラスに emit される `private static readonly` フィールド名もこのモデルに含めること。

```csharp
internal sealed record MapperMethodModel(
    string Namespace,
    string ClassName,
    bool   IsValueType,
    Accessibility Accessibility,
    string MethodName,
    MapperKind Kind,           // Reader / Writer
    MapperShape Shape,         // In-place / New / WriteSpan / WriteAlloc
    string TargetTypeFqn,
    string? ProfileTypeFqn,
    int MethodIndex,           // 同一 partial class 内の連番 (Converter フィールド命名に使用)
    LayoutModel Layout,
    EquatableArray<MemberMappingModel> Members,
    EquatableArray<TypeMappingModel> TypeMappings,
    EquatableArray<DiagnosticInfo> Errors);

internal sealed record LayoutModel(
    int Size,
    byte Filler,
    byte NullFiller,
    bool UseDelimiter,
    EquatableArray<byte> Delimiter,
    bool AutoFiller,
    bool Validation);

internal sealed record MemberMappingModel(
    string PropertyName,
    string PropertyTypeFqn,
    bool IsNullable,           // 参照型 nullable / Nullable<T> いずれも true
    int Offset,
    int Size,
    int PropertyIndex,         // メソッド内のメンバ連番 (Converter フィールド命名に使用)
    ConverterCallModel Converter);

internal sealed record TypeMappingModel(
    int Offset,
    int Size,
    TypeMappingKind Kind,      // Constant / Filler
    EquatableArray<byte> Constant,
    byte Filler);

// すべての Converter 呼び出しを表す唯一のモデル
internal sealed record ConverterCallModel(
    string ConverterTypeFqn,                     // 閉じた構築型の完全修飾名 (例: "global::Smart.IO.ByteMapper.Converters.BinaryConverter<int>")
    string FieldName,                            // 生成フィールド名 (例: "Converter0_3")
    EquatableArray<string> CtorArgExpressions,   // new <ConverterTypeFqn>(<ここ>) のコンストラクタ引数式
    SizeKind SizeKind,                           // Const / Instance
    int? ConstSize);                             // SizeKind=Const のとき (BinaryConverter<int> 等)

internal enum SizeKind { Const, Instance }      // Instance = フィールド経由で <field>.Size を参照
```

モデル仕様の要点(必達):
- `ConverterTypeFqn` は **閉じた構築型の FQN**。`BinaryConverter<int>` も `MyEndianConverter<long>` も `MyHexConverter` も同じ形式で格納すること。
- `FieldName` は **メソッド連番 + プロパティ連番** で組み立てること: `$"Converter{MethodIndex}_{PropertyIndex}"`。例: メソッド 0 / プロパティ 3 → `Converter0_3`。
- `CtorArgExpressions` は **生成コードに埋め込むコンストラクタ引数式の文字列**(例: `"32"`, `"20127"`, `"true"`, `"global::Smart.IO.ByteMapper.Padding.Right"`, `"(byte)0x20"`)。Generator 側で `TypedConstant` → C# リテラル変換を一度行い、ここに保存しておくこと。
- `SizeKind = Const` の場合は生成コードに `<ConstSize>` 整数リテラルを埋め込み、`Instance` の場合は `<FieldName>.Size` を埋め込むこと。
- 旧 `BinaryConverterModel` 等の specialized record は **持たせないこと**。Generator のコード生成パスは「フィールド宣言 `new <ConverterTypeFqn>(<ctor args>)` を書く」「呼び出し `<FieldName>.Read(buffer.Slice(...))` を書く」だけで完結させること。
- `EquatableArray<T>` を全フィールドで使い、`record` の値等価性を Incremental パイプラインのキャッシュキーとして機能させること(Smart.Mapper と同じ流儀)。

### 7.5 出力構造(必須仕様)
- ファイル名: `<Namespace>_<ClassName>.g.cs`(`__Reference/Mapper` と同じ `MakeFilename` を流用すること)。
- 同一 namespace + クラスにある複数メソッドは 1 ファイルにまとめること。
- 先頭に `// <auto-generated/>` と `#nullable enable` を付けること。

雛形(以下の形に従うこと):
```csharp
// <auto-generated />
#nullable enable

namespace MyApp.Mapping;

partial class RecordMappers
{
    // ── Converter インスタンスフィールド (メソッド連番 + プロパティ連番) ──
    // [MapBinary<int>(0)] (メソッド 0 / プロパティ 0)
    private static readonly global::Smart.IO.ByteMapper.Converters.BinaryConverter<int> Converter0_0
        = new(global::Smart.IO.ByteMapper.Endian.Big);
    // [MapText(4, 32)] (メソッド 0 / プロパティ 1)
    private static readonly global::Smart.IO.ByteMapper.Converters.TextConverter Converter0_1
        = new(32, 20127, true, global::Smart.IO.ByteMapper.Padding.Right, (byte)0x20);
    // … 同じメソッドの他プロパティ用フィールドが続く …

    // メソッド 1 (Write) のフィールド
    private static readonly global::Smart.IO.ByteMapper.Converters.BinaryConverter<int> Converter1_0
        = new(global::Smart.IO.ByteMapper.Endian.Big);
    private static readonly global::Smart.IO.ByteMapper.Converters.TextConverter Converter1_1
        = new(32, 20127, true, global::Smart.IO.ByteMapper.Padding.Right, (byte)0x20);

    // ── Read / Write 本体 ──
    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static partial void Read(global::System.ReadOnlySpan<byte> buffer, global::MyApp.OrderRecord target)
    {
        target.Id   = Converter0_0.Read(buffer.Slice(0, 4));
        target.Name = Converter0_1.Read(buffer.Slice(4, 32));
        // …
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static partial void Write(global::MyApp.OrderRecord source, global::System.Span<byte> buffer)
    {
        Converter1_0.Write(buffer.Slice(0, 4), source.Id);
        Converter1_1.Write(buffer.Slice(4, 32), source.Name);
        // …
    }
}
```

出力規約(必達):
- すべての Converter 呼び出しを `<FieldName>.Read(buffer.Slice(offset, size))` / `<FieldName>.Write(buffer.Slice(offset, size), value)` の **2 引数の短い call site** に統一すること。組み込み属性とユーザ拡張属性で出力構造は同じとすること。
- Encoding / IFormatProvider 等の高コストオブジェクトは **フィールド初期化 1 回** で解決させ、Read/Write 本体には現れさせないこと。
- Generator はモデルの `ConverterCallModel` から `FieldName` と `ConverterTypeFqn` と `CtorArgExpressions` をそのまま emit するだけで完結させること。
- 同一プロパティでも **Read メソッドと Write メソッドで別フィールド**(`Converter0_*` / `Converter1_*`)とすること。共有しない理由は Generator の単純化(連番がメソッド境界でリセットされるため)。Converter は軽量なオブジェクトなので、フィールド 2 個分のメモリ重複は実用上無視できる。

### 7.6 SourceBuilder
- `SourceGenerateHelper.SourceBuilder` を使用し、`Indent / Append / BeginScope / EndScope` で構築すること。
- 同パッケージの `Result<T>` / `EquatableArray<T>` / `DiagnosticInfo` を活用すること。

---

## 8. 生成コードの性能要件

### 8.1 共通必達原則
- **boxing 撲滅**: Converter は generic なインスタンスメソッド `Read`/`Write` を直接公開し、`object` を経由しないこと。値型はそのまま値型として戻り値・引数になること。
- **設定値の前解決**: `Encoding` / `IFormatProvider` / フォーマット文字列など、生成コストが大きいオブジェクトは **Converter コンストラクタで 1 回だけ解決** し、`readonly` フィールドに保持すること。Read/Write の毎呼び出しでは再評価しないこと。
- **per-call allocation = 0**: 文字列 / `byte[]` プロパティを除き、Converter 内部・生成コードの両方で確保ゼロを達成すること。
- **インライン化**: 生成された partial メソッドに `[MethodImpl(AggressiveInlining)]` を付けること。Converter 側の `Read`/`Write` も短い実装の場合は同様にマークすること。
- **Span API のみ**: 生成コードは `buffer.Slice(offset, size)` で対象領域を切り出し、Converter のインスタンスメソッドへ転送すること。Converter 内部では `BinaryPrimitives.Read/Write*`・`Encoding.GetString`/`GetBytes` 等を直接呼ぶこと。
- **null filler**: Target が `null` のケースは Write メソッド初期段に `if (source is null) { buffer.Slice(0, Size).Fill(nullFiller); return; }` を生成すること(参照型のみ)。プロパティ単位でも同様に対応すること。

### 8.2 Converter インスタンス呼び出しコスト要件
- 組み込み・拡張ともに同じ呼び出し形 `<FieldName>.Read(span)` / `<FieldName>.Write(span, value)` とすること。引数 2 個の短い call site とすること。
- フィールドは `private static readonly` で `sealed class` を保持すること。JIT/AOT は仮想呼び出しではなく非仮想呼び出し相当として最適化(devirtualize)し、`this` ポインタ経由でフィールドを参照すること。
- `BinaryConverter<T>` のような値型ジェネリックは CLR / NativeAOT のジェネリック特殊化により、未使用の `typeof(T)` 分岐がデッドコード除去されることを前提とすること。
- 旧 `IMapConverter` の VTable 呼び出し + `object` ボックス化のオーバーヘッドはすべて消滅させること。

### 8.3 Encoding 解決(前解決の効果・必達)
- `TextConverter` / `NumberTextConverter<T>` / `DateTimeTextConverter<T>` のコンストラクタで `Encoding.GetEncoding(codePage)` を **1 回だけ** 呼び、結果を `readonly Encoding encoding` フィールドに格納すること。
- 既知の `codePage`(ASCII / UTF-8 / Shift_JIS など) はコンストラクタ内の switch で `Encoding.ASCII` 等のシングルトンを返すショートカットを置き、`GetEncoding` のディクショナリ引きを回避すること。
- 受け入れ基準: 100 万件 × 10 文字列フィールド規模で `GetEncoding` 呼び出し回数 = 0 (静的初期化の 1 回のみ) を達成すること。

### 8.4 数値テキスト/日付テキスト
- `NumberTextConverter<T>` / `DateTimeTextConverter<T>` 内部で `int.TryParse(span, NumberStyles, CultureInfo, out value)` のような Span 受付 API を使用すること。
- 書き込みは `Utf8Formatter.TryFormat` を採用すること(ASCII 経路)。多バイト Encoding 経路では `stackalloc char[…]` 経由でフォールバックすること。
- `IFormatProvider` (`Culture == Invariant` なら `CultureInfo.InvariantCulture`、`Current` なら `CultureInfo.CurrentCulture`) はコンストラクタでフィールドに保持すること。
- フォーマット文字列はコンパイル時に既知のため、生成コードから Converter コンストラクタの引数として文字列リテラルを渡し、Converter 側でフィールド保持すること。

### 8.5 Decimal/Binary
- `BinaryConverter<T>` 内部で `typeof(T)` の switch を行い、`int`/`long`/`short`/...・`float`/`double` ごとに `BinaryPrimitives` の各メソッドを呼ぶこと。`decimal` は `BinaryPrimitives.ReadInt32*` を 4 回呼んで `decimal.GetBits(...)` / `new decimal(...)` で組み立てること(既存ロジック移植)。
- Double/Float は `BitConverter.UInt64BitsToDouble` 等の安全 API を優先すること(`BytesHelper.Int64ToDouble` の `Unsafe.As` を脱却すること)。
- `Endian` 設定はコンストラクタでフィールドに格納し、メソッド内分岐で参照すること。

### 8.6 配列 (`MapArray`)
- §6.10 のとおり、専用 Converter は持たせず Generator が `for` ループを emit すること。要素 Converter のインスタンスは **要素プロパティ専用の `static readonly` フィールド** として保持すること。
- 要素呼び出しは `sealed class` の static フィールド経由なので JIT/AOT で非仮想化され、ループ内で直接 inline 化されることを前提とすること。

### 8.7 検証コード
- `ValidateLayout=true` のときも、生成コードに長さチェックを `if (buffer.Length < Size) throw new ByteMapperException(...)` の **1 行** だけ埋め込むこと(範囲外を確実に検出するため)。
- それ以外は範囲チェックは JIT に任せて生成コードから外すこと。Converter 側でも追加検証は最低限とすること。

---

## 9. Diagnostics

ID プレフィクスは `SBM`(Smart ByteMapper)に統一すること。

| ID | 重要度 | 概要 |
| --- | --- | --- |
| SBM0001 | Error | メソッドが `static partial` でない |
| SBM0002 | Error | サポートしない引数シグネチャ |
| SBM0003 | Error | Target 型に `[Map]` がない / Profile も指定されていない |
| SBM0004 | Error | Offset / Length が負 |
| SBM0005 | Error | 属性とプロパティ型が一致しない |
| SBM0006 | Warning | 範囲オーバーラップ(`ValidateLayout=true` 時のみ) |
| SBM0007 | Error | レイアウトが `[Map(size)]` を超過 |
| SBM0008 | Error | 未知の型(`MapBinary` で対応外) |
| SBM0009 | Error | Encoding / Culture 名が無効 |
| SBM0010 | Error | カスタム Converter が契約を満たさない |
| SBM0011 | Error | Profile 型に対応するプロパティが Target に無い |
| SBM0012 | Error | Profile / Target のプロパティ型が一致しない |
| SBM0013 | Error | Profile 型に `[Map]` が無い |
| SBM0014 | Error | 戻り値版 (`T Read(...)` / `byte[] Write(...)`) で `T` がインスタンス化不能 |
| SBM0015 | Error | 重複した `MapConstant`/`MapFiller`/メンバ属性が同一範囲に存在 |
| SBM0016 | Warning | 必須プロパティが未マッピング |
| SBM0017 | Warning | 拡張 Converter の引数が不足/余剰 |
| SBM0018 | Info | 配列展開数が多い(ループ展開を行わない) |

要件: `DiagnosticSuppressor`(Smart.Mapper.Generator の `MapperDiagnosticSuppressor` 相当)で、生成コードがアサインする non-nullable プロパティに対する `CS8618` を必要に応じ抑制すること。

---

## 10. ランタイム共有 API

### 10.1 共通型 (`Smart.IO.ByteMapper`)(必須実装)

```csharp
namespace Smart.IO.ByteMapper;

public enum Endian { Big, Little }
public enum Padding { Left, Right }
public enum Culture { Invariant, Current }

public sealed class ByteMapperException : Exception
{
    public ByteMapperException() { }
    public ByteMapperException(string message) : base(message) { }
    public ByteMapperException(string message, Exception inner) : base(message, inner) { }
}

// 属性 (基底 + 組み込み): §4 / §6 のとおり
// 生成コードから呼び出される低レベルユーティリティ
public static class ByteMapperHelpers
{
    public static void Fill(Span<byte> destination, byte value);
    public static void TrimRange(ReadOnlySpan<byte> buffer, ref int start, ref int size, Padding padding, byte filler);
    public static void CopyWithPadding(ReadOnlySpan<byte> source, Span<byte> destination, int length, Padding padding, byte filler);
}
```

### 10.2 組み込み Converter (`Smart.IO.ByteMapper.Converters`)(必須実装)

§6.3 のインスタンス契約に従う組み込み Converter 群を実装すること。生成コードからも、また同じシグネチャで **直接ユーザコードからも `new` して使える**(ライブラリ的に使える)状態にすること。

```csharp
namespace Smart.IO.ByteMapper.Converters;

public sealed class BinaryConverter<T>
    where T : unmanaged
{
    private readonly Endian endian;
    public BinaryConverter(Endian endian) { this.endian = endian; }

    public static readonly int Size = global::System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
    public T Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, T value);
}

public sealed class ByteConverter
{
    public static readonly ByteConverter Instance = new();
    public ByteConverter() { }

    public const int Size = 1;
    public byte Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, byte value);
}

public sealed class BytesConverter
{
    private readonly byte filler;
    public BytesConverter(int length, byte filler) { Size = length; this.filler = filler; }

    public int Size { get; }
    public byte[] Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, byte[] value);
}

public sealed class TextConverter
{
    private readonly System.Text.Encoding encoding;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;

    public TextConverter(int length, int codePage, bool trim, Padding padding, byte filler)
    {
        Size = length;
        this.encoding = ResolveEncoding(codePage);   // ← 前解決
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    public int Size { get; }
    public string Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, string value);

    private static System.Text.Encoding ResolveEncoding(int codePage) => codePage switch
    {
        20127 => System.Text.Encoding.ASCII,
        65001 => System.Text.Encoding.UTF8,
        _     => System.Text.Encoding.GetEncoding(codePage)
    };
}

public sealed class BooleanConverter
{
    private readonly byte trueValue;
    private readonly byte falseValue;
    private readonly byte nullValue;
    public BooleanConverter(byte trueValue, byte falseValue, byte nullValue) { /* … */ }

    public const int Size = 1;
    public bool? Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, bool? value);
}

public sealed class NumberTextConverter<T>
{
    public NumberTextConverter(
        int length,
        string? format,
        int codePage,
        bool trim,
        Padding padding,
        byte filler,
        System.Globalization.NumberStyles style,
        Culture culture);

    public int Size { get; }
    public T Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, T value);
}

public sealed class DateTimeTextConverter<T>
{
    public DateTimeTextConverter(
        int length,
        string format,
        int codePage,
        System.Globalization.DateTimeStyles style,
        Culture culture,
        byte filler);

    public int Size { get; }
    public T Read(ReadOnlySpan<byte> source);
    public void Write(Span<byte> destination, T value);
}
```

実装ポイント:
- `BinaryConverter<T>` などジェネリック化された Converter は、`typeof(T) == typeof(int)` 形式の switch で `BinaryPrimitives` の型別 API を呼ぶこと。CLR/NativeAOT のジェネリック特殊化により、各クローズドコンストラクト型に対しては未使用分岐がデッドコード化される。
- 配列は専用 Converter を設けず、§6.10 のとおり Generator が `for` ループを emit すること。

### 10.3 旧 `IMapConverter` からの移行(必須対応)

旧版の `IMapConverter`(`object Read(byte[], int)` / `void Write(byte[], int, object)` のインスタンス契約)は **廃止すること**。§6.3 の **インスタンス契約 + `ByteMapperConverterAttribute<TConverter>`** に置き換えること。

| 旧 | 新 | 主な変更 |
| --- | --- | --- |
| `interface IMapConverter` | `sealed class <Name>Converter` | インタフェースなし。`sealed class` 規約のみ |
| `object Read(byte[], int)` | `T Read(ReadOnlySpan<byte> source)` | Span ベース・generic 化で boxing 解消 |
| `void Write(byte[], int, object)` | `void Write(Span<byte> destination, T value)` | 同上 |
| 設定値は `IMapConverterBuilder` で組立 | 設定値は **コンストラクタ引数** で受け取り、フィールド保持 | リフレクションも実行時組立もない |
| `byte[]` + index 指定 | `Span<byte>`(Slice 済) | Generator 側で offset/size を切り出して渡す |

旧版の Unsafe ヘルパ (`BytesHelper.Int64ToDouble` 等) は原則不要となる(必要箇所は `BitConverter` の Bits 系 API で代替すること)。

---

## 11. テスト要件

以下のテストを必須実装とすること。

1. **ゴールデンファイルテスト**: 既知の入出力バイト列で Read/Write 結果を検証すること(`Smart.IO.ByteMapper.Tests`)。
2. **Generator スナップショットテスト**: 既存 Smart.Mapper.Generator.Tests と同様の `CSharpSourceGeneratorTest` パターンで、生成された `.g.cs` を比較すること。
3. **AOT 互換テスト**: `Smart.IO.ByteMapper.AotTests` を `dotnet publish -c Release -r win-x64 --self-contained` で公開し、警告 0 を CI で確認すること。
4. **ベンチマーク**: `__Reference/ByteMapper.Benchmark` を参考に、Read/Write、配列、文字列パッディングを計測すること。**既存リフレクション版より速いか** を判定基準とすること。

---

## 12. 開発フェーズ

以下のフェーズ順で実装を進めること。

| フェーズ | 内容 |
| --- | --- |
| 0. 仕様確定 | 本ドキュメントレビュー / 既存ユーザの移行ガイド草案 |
| 1. コア(Binary/Byte/Boolean/Text) | ランタイム属性・Generator パイプライン・基本コード生成・Diagnostic 主要 ID |
| 2. テキスト系 | `MapNumberText`, `MapDateTimeText`, Encoding/Culture 解決 |
| 3. Bytes / Array | 配列ループ生成、固定長 Bytes |
| 4. レイアウト | `MapConstant`, `MapFiller`, AutoFiller, Delimiter, レイアウト検証 |
| 5. Profile | 既存クラス向け shadow class 方式 |
| 6. 拡張属性 | `ByteMapperConverter` ベースのユーザ拡張、契約検証 |
| 7. パフォーマンス最適化 | Span pipeline 強化、ベンチマーク、AOT テスト |
| 8. 周辺 | `AspNetCore`/`Options` 連携の再実装(必要なら) |

---

## 13. 残課題・要決定事項

実装着手時に決着させること。

1. **`UseDelimiter` のデフォルト**: 既存版は固定 `CRLF`。生成コードで明示する仕様にするか、`[ByteMapperDefaults]` で上書きするか。→ 推奨: 既定 CRLF、必要なら型属性で `Delimiter = new byte[]{...}` を許可。
2. **`StringComparison` の扱い**: Profile/Target のプロパティ名照合。当面 `Ordinal` 固定で良いか。→ 推奨: 固定。要望出たら追加。
3. **`record`/`record struct` 対応**: 一次コンストラクタ経由でしか生成できないケース(set がない init-only)。→ §3.2 の `T Read(...)` 戻り値版を必須化することで対応。
4. **null 安全**: `string` プロパティが nullable な場合、Read 時に `string.Empty` を入れるか `null` のままにするか。→ プロパティの nullable annotation に従う(`string` → empty、`string?` → null も許容)。
5. **メソッド命名の一貫性**: ユーザ要望のサンプルでは `Write(...)` がすべての例で使われていたが、これは「メソッド名は自由」という意図と解釈し、本仕様では Read 系/Write 系それぞれで自由命名とする。
6. **既定値の単位**: `[ByteMapperDefaults]` をアセンブリ属性として導入するか、`Directory.Build.props` のグローバル設定 + `[GeneratedOption]` で渡すか。→ 推奨: アセンブリ属性。
7. **`MapArrayAttribute<TElementAttribute>` の要素設定受け渡し**: 要素属性のプロパティを `MapArrayAttribute` から指定する手段が無いため、当面は要素プロパティ専用属性 (`MapBinaryArrayAttribute<T>` 等) の合成属性方式とする。

---

## 付録 A. 設計選択肢比較(参考情報)

本付録は設計判断の根拠を参照したい場合の参考資料。実装時に従うべき結論は本文に記載する。

### A.1 プロファイル方式の比較

| 案 | 概要 | 採否 |
| --- | --- | --- |
| A. 「shadow class」方式(本仕様) | Profile 型は Target と同じ shape を持つ POCO。属性は Profile 側に書く | **採用** |
| B. 専用 `[MapMember("PropName", ...)]` 属性方式 | Profile クラスに `[MapMember]` 列挙 | プロパティ毎の型確認が困難になりがちで保留 |
| C. アセンブリ属性で `[Map(typeof(Target))]` 形式 | 中心集約型。重複指定の衝突管理が必要 | 大規模システム向けに将来追加検討 |

A 案採用の理由: 既存属性をそのまま再利用でき、Generator 側のロジックも単純化できるため。

### A.2 Converter 契約方式の比較

#### A.2.1 属性 ↔ Converter の結合方式

| 案 | 概要 | 採否 |
| --- | --- | --- |
| A. メタ属性方式 (`[ByteMapperConverter(typeof(...))]` を属性クラス自体に付ける) | 旧設計案。Converter ↔ 属性 を 1 行で紐付け | 不採用(二度手間) |
| B. メタ属性方式 (`[ByteMapperConverter(typeof(...))]` をプロパティに付ける) | 使う側で毎回書く必要があり煩雑 | 不採用 |
| **C. ジェネリック基底属性 (`ByteMapperConverterAttribute<TConverter>`) を継承** | 派生属性を 1 つ書けば、使用側は素朴に `[MapHex(...)]` だけ。Generator はジェネリック型引数を読むだけ。**組み込み属性も同じ仕組み** | **採用** |
| D. 規約方式(属性名 = Converter 名) | フリッキーで衝突しやすい | 不採用 |
| E. マクロ展開型(組み合わせ属性の別名宣言) | 既存属性の組合せに名前だけ付けたい要件があれば追加。フェーズ 2 で検討 | 追加候補 |

#### A.2.2 Converter の実行モデル

| 案 | 概要 | 採否 |
| --- | --- | --- |
| α. `static` メソッド + 毎回引数渡し | `Type.Read(span, length, codePage, ...)` の形式。引数は属性から定数畳み込みされ JIT インライン化される | 不採用 |
| **β. `sealed class` インスタンス + Mapper 側 `static readonly` フィールド** | コンストラクタで `Encoding`/`IFormatProvider` 等を **前解決**。Read/Write はフィールド経由 1 引数渡し | **採用** |
| γ. 旧 `IMapConverter` インタフェース(`object` 経由) | Boxing と VTable コールが避けられない | 不採用 |
| δ. C# 11 `static abstract` インタフェース | 引数並びが固定化され、属性毎の自由なパラメータが渡せない | 不採用 |

β 採用の理由: `TextConverter` / `NumberTextConverter` / `DateTimeTextConverter` での `Encoding.GetEncoding(codePage)` / `CultureInfo` 解決を **1 回だけ** に圧縮できる。100 万レコード × 10 文字列フィールドで実測オーダのオーバーヘッド差が出る。`BinaryConverter` のように設定が軽い Converter では α/β に大差はないが、契約を統一する利点が勝る。

---

## 付録 B. 既存 API との対応表(移行参考)

| 既存(リフレクション版) | 新版(Generator) |
| --- | --- |
| `MapperFactoryConfig → factory.Create<T>()` | partial メソッド宣言で代替 |
| `ITypeMapper<T>.FromByte(buffer, target)` | `[ByteReader] static partial void M(ReadOnlySpan<byte>, T)` |
| `ITypeMapper<T>.ToByte(buffer, target)` | `[ByteWriter] static partial void M(T, Span<byte>)` |
| `MapperProfile` クラスでの Fluent 定義 | `Profile = typeof(...)` + 属性ベース shadow class |
| `IMapConverter` プラグイン(インスタンス契約・`object` 経由・boxing) | **インスタンス契約 (§6.3) + `ByteMapperConverterAttribute<TConverter>` 派生**。generic な `Read`/`Write` で boxing 消滅。組み込み Converter (`BinaryConverter<T>` 等) もこの形に統一 |
| `IMapConverterBuilder`(リフレクション組立) | Generator が解析時に直接 `ConverterCallModel` を構築 → 生成コードで `new <Converter>(...)` ── 実行時組立は無し |
| 各種組み込み Converter (`*BinaryConverter`, `TextConverter`, …) | `Smart.IO.ByteMapper.Converters.*` 名前空間に **`sealed class`** として再実装。`Encoding`/`IFormatProvider` をコンストラクタで前解決 |
| `Parameter`(実行時グローバル設定) | `[ByteMapperDefaults]`(アセンブリ属性) |
| `ByteMapperException`(実行時例外) | 同名で残置(構築エラーは Diagnostic に置換) |

---

## 付録 C. アーキテクチャまとめ(チェックリスト)

実装完了時、以下の項目がすべて満たされていることを確認すること。

- [ ] Source Generator が `IIncrementalGenerator + SourceGenerateHelper + EquatableArray<T>` パターンを踏襲している。
- [ ] ユーザ API は `[ByteReader]`/`[ByteWriter]` + `static partial` の 4 シグネチャをすべてサポートしている。
- [ ] Target 型は具象、Profile 指定で既存クラスにも対応している。
- [ ] **変換処理は必ず固有の Converter クラスを通る**(`BinaryConverter<T>` / `TextConverter` / `BooleanConverter` / `BytesConverter` / `NumberTextConverter<T>` / `DateTimeTextConverter<T>` および拡張 Converter)。
- [ ] 組み込み・拡張のいずれも §6.3 の **インスタンス契約**(`sealed class` + コンストラクタで設定値前解決 + generic `Read`/`Write`)に従っている。
- [ ] **組み込み属性も拡張属性も同じ仕組み**(`ByteMapperConverterAttribute<TConverter>` 継承)で結びついている。Generator にとって両者は区別なく単一パスで処理される。
- [ ] 生成コードは Mapper クラスに **`private static readonly <Converter> Converter<M>_<P> = new(...)` フィールド** を emit し、Read/Write 本体は `<FieldName>.Read(buffer.Slice(...))` / `<FieldName>.Write(buffer.Slice(...), value)` の **2 引数呼び出し** に統一されている。
- [ ] `Encoding` / `IFormatProvider` などの高コスト解決は **コンストラクタで 1 回だけ** に圧縮されている。
- [ ] boxing・VTable コール・`Activator`/`MethodInfo` は完全排除されている(AOT/Trim 完全安全)。
- [ ] ランタイム例外(`ByteMapperException`)は最終手段とし、構築時不整合は Diagnostic で検出されている。
- [ ] AGENTS.md の規約(`_` プレフィックス不可、ビルド警告ゼロ)を遵守している。
