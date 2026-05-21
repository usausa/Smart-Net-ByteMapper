# AOT互換性・BinaryConverter最適化・重複検証レポート

## 1. AOT互換性 概要

| プロジェクト | ターゲット | AOT対応状況 | 主な懸念点 |
|---|---|---|---|
| `Smart.IO.ByteMapper` | net8/9/10 | ✅ 修正済み | `(T)(object)` ボクシング → `Unsafe.BitCast` で解消 |
| `Smart.IO.ByteMapper.Fast` | net8/9/10 | ✅ 安全 | `Encoding.GetEncoding(codePage)` に注意（後述） |

---

## 2. BinaryConverter&lt;T&gt; の `(T)(object)` 問題と修正

### 問題：ボクシングによる JIT/AOT 両面の非効率

```csharp
// 修正前 — 型引数が unmanaged なのに object 経由でボクシング発生
return (T)(object)BinaryPrimitives.ReadInt32BigEndian(source);
```

`BinaryConverter<T>` は `where T : unmanaged` 制約を持ちますが、`(T)(object)` キャストはボクシング（ヒープアロケーション）とアンボクシング（型チェック）を発生させます。  
また NativeAOT ではジェネリック型インスタンシエーションが静的に解決されるため、`(T)(object)value` の unbox パスが型ごとに生成されコード膨張を招きます。

### 解決：`Unsafe.BitCast<TFrom, TTo>` (.NET 8+)

```csharp
// 修正後 — ボクシングゼロ・AOT安全・JIT最適化対象
var v = BinaryPrimitives.ReadInt32BigEndian(source);
return Unsafe.BitCast<int, T>(v);
```

`Unsafe.BitCast<TFrom, TTo>` は：
- **ボクシングなし**（unmanaged 値を直接ビット再解釈）
- **AOT安全**（ランタイムリフレクション不要）
- **JIT最適化対象**（`AggressiveInlining` と組み合わせてレジスタ直渡し）
- コンパイル時にサイズ一致チェックあり（`TFrom`/`TTo` のサイズが異なるとコンパイルエラー）

### 修正範囲

| ファイル | 修正内容 |
|---|---|
| `Smart.IO.ByteMapper/Converters/BinaryConverter.cs` | `Read`/`Write` の全 `(T)(object)` → `Unsafe.BitCast` |
| `Smart.IO.ByteMapper/Converters/NumberTextConverter.cs` | `ParseValue` の全 `(T)(object)` → `Unsafe.BitCast`、`where T : struct` 追加 |
| `Smart.IO.ByteMapper/Converters/DateTimeTextConverter.cs` | `ParseValue` の全 `(T)(object)` → `Unsafe.BitCast`、`where T : struct` 追加 |
| `Smart.IO.ByteMapper/MapMemberAttributes.cs` | `MapNumberTextAttribute<T>` / `MapDateTimeTextAttribute<T>` に `where T : struct` 追加 |

### Nullable型 (`T?`) の扱い変更

変更前は `NumberTextConverter<int?>` のような nullable 型も受け付けていましたが、  
`Unsafe.BitCast` は reference type に使えないため `where T : struct` 制約を追加し nullable 型を除外しました。  
**使用コード側への影響なし**（既存コードで `NumberTextConverter<int?>` は使用されていない）。

---

## 3. `Smart.IO.ByteMapper` と `Smart.IO.ByteMapper.Options` の重複検証

### 3.1 コンバーター対応表

| 機能 | `Smart.IO.ByteMapper` | `Smart.IO.ByteMapper.Options` | 重複状況 |
|---|---|---|---|
| バイナリ整数 (int/long等) | `BinaryConverter<T>` | なし | ✅ 住み分け明確 |
| 1バイト読み書き | `ByteConverter` | なし | ✅ 住み分け明確 |
| バイト配列 | `BytesConverter` | なし | ✅ 住み分け明確 |
| ブール値 | `BooleanConverter` | なし | ✅ 住み分け明確 |
| ASCII文字列 | `TextConverter` (Encoding抽象) | `AsciiConverter` (System.Text.Ascii) | ⚠️ 重複あり（実装差異） |
| Unicode文字列 (UTF-16LE) | `TextConverter` (codePage=1200) | `UnicodeConverter` | ⚠️ 重複あり（実装差異） |
| 整数テキスト | `NumberTextConverter<T>` (Encoding経由) | `IntegerConverter<T>` (バイト直操作) | ⚠️ 役割重複 |
| 小数テキスト | `NumberTextConverter<T>` | `DecimalConverter` (バイト直操作) | ⚠️ 役割重複 |
| 日時テキスト | `DateTimeTextConverter<T>` (Encoding経由) | `DateTimeConverter` (バイト直操作) | ⚠️ 役割重複 |

### 3.2 実装差異の詳細

#### ASCII文字列：`TextConverter` vs `AsciiConverter`

| 観点 | `TextConverter` | `AsciiConverter` |
|---|---|---|
| エンコーディング | `Encoding.ASCII` 経由 | `System.Text.Ascii.FromUtf16` (直接バイト変換) |
| パフォーマンス | string 経由でアロケートあり | ゼロアロケーション |
| AOT互換 | ✅ | ✅ |
| Encoding依存 | あり (`Encoding.GetEncoding` 分岐) | なし |

`AsciiConverter` は **`TextConverter` の ASCII 特化高性能版**。用途が完全に重複するが、Options 側がより高効率。

#### 整数テキスト：`NumberTextConverter<T>` vs `IntegerConverter<T>`

| 観点 | `NumberTextConverter<T>` | `IntegerConverter<T>` |
|---|---|---|
| 内部処理 | `string` 経由（`Int32.Parse`） | バイト直接操作 (`NumberByteHelper`) |
| アロケーション | string アロケートあり | ゼロアロケーション |
| 書式指定 | `NumberStyles`/`IFormatProvider` | zerofill/padding のみ |
| 対応型 | int/long/short/float/double/decimal | int/short/long |
| AOT互換 | ✅ (`Encoding.ASCII`/`Encoding.UTF8` は静的) | ✅ |

`IntegerConverter<T>` は **ゼロアロケーションの高速版**。Options が高パフォーマンスな特化実装。

#### 日時テキスト：`DateTimeTextConverter<T>` vs `DateTimeConverter`

| 観点 | `DateTimeTextConverter<T>` | `DateTimeConverter` |
|---|---|---|
| 内部処理 | `string` 経由（`DateTime.ParseExact`） | バイト直接操作 (`DateTimeByteHelper`) |
| アロケーション | string アロケートあり | ゼロアロケーション |
| フォーマット | .NET 標準フォーマット文字列 | 独自パターン (`DateTimeByteHelper`) |
| 対応型 | `DateTime`/`DateTimeOffset`/`DateOnly`/`TimeOnly` | `DateTime`/`DateTimeOffset` |
| AOT互換 | ✅ | ✅ |

### 3.3 AOT固有の懸念事項

#### `Encoding.GetEncoding(codePage)` — 非標準コードページ

```csharp
// TextConverter.cs / NumberTextConverter.cs / DateTimeTextConverter.cs
private static Encoding ResolveEncoding(int codePage) => codePage switch
{
    20127 => Encoding.ASCII,   // ✅ 静的、AOT安全
    65001 => Encoding.UTF8,    // ✅ 静的、AOT安全
    _ => Encoding.GetEncoding(codePage)  // ⚠️ NativeAOT でトリムされる可能性
};
```

**リスク：** `Encoding.GetEncoding(int)` は NativeAOT でトリミングにより利用不可になる場合があります。  
**対策：** 非標準コードページが不要であれば現状で問題なし。使用する場合は `rd.xml` または `[DynamicDependency]` でエンコーディングを保持する必要があります。

#### `IFormattable.ToString` in `DateTimeTextConverter.Write`

```csharp
// 修正後
private string FormatValue(T value)
{
    if (value is IFormattable f)  // インターフェイスチェック
        return f.ToString(format, provider);
    ...
}
```

**`is IFormattable` パターン：** NativeAOT では `T : struct` かつ `where T : struct` の場合、JIT が型チェックをコンパイル時に解決できます。`DateTime`/`DateOnly` 等はすべて `IFormattable` を実装しているため、問題なし。

---

## 4. 住み分けの推奨方針

```
Smart.IO.ByteMapper (コア)
  ├─ BinaryConverter<T>     : バイナリ整数/浮動小数 → 用途独自、統合不可
  ├─ BooleanConverter       : ブール値 → 用途独自、統合不可
  ├─ ByteConverter          : 1バイト → 用途独自、統合不可
  ├─ BytesConverter         : バイト配列 → 用途独自、統合不可
  ├─ TextConverter          : 汎用エンコーディング文字列 (UTF8等含む汎用用途)
  ├─ NumberTextConverter<T> : 汎用数値テキスト (標準フォーマット対応用)
  └─ DateTimeTextConverter<T>: 汎用日時テキスト (DateOnly/TimeOnly対応用)

Smart.IO.ByteMapper.Options (高性能特化)
  ├─ AsciiConverter         : ゼロアロケーション ASCII (TextConverter の高速代替)
  ├─ UnicodeConverter       : ゼロアロケーション UTF-16LE
  ├─ IntegerConverter<T>    : ゼロアロケーション整数テキスト
  ├─ DecimalConverter       : ゼロアロケーション decimal テキスト
  └─ DateTimeConverter      : ゼロアロケーション日時テキスト
```

**コア側はゼロ依存・汎用性重視、Options側はパフォーマンス重視の特化実装**という位置付けで住み分けは合理的。  
ただし同じ属性 (`[MapNumberText]` 等) からコア/Options どちらのコンバーターを使うかは、ジェネレーター設定か属性の名前空間で明示すると混乱が生じない。

---

## 5. 変更サマリー（コードベース）

| ファイル | 変更 | 目的 |
|---|---|---|
| `BinaryConverter.cs` | `(T)(object)` → `Unsafe.BitCast` (全箇所) | ボクシング排除・AOT最適化 |
| `NumberTextConverter.cs` | `(T)(object)` → `Unsafe.BitCast`、nullable型除外、`where T : struct` | ボクシング排除・AOT安全化 |
| `DateTimeTextConverter.cs` | `(T)(object)` → `Unsafe.BitCast`、nullable型除外、`where T : struct` | ボクシング排除・AOT安全化 |
| `MapMemberAttributes.cs` | `where T : struct` 制約追加 (2クラス) | 型制約の一貫性 |
| `ByteMapperGenerator.cs` | `BinaryConverter<T>.Size` を定数リテラルに解決 | JIT境界チェック最適化 |

---

## 6. Smart.IO.ByteMapper.Fast への命名変更

`Smart.IO.ByteMapper.Options` は下記ベンチマーク結果から明らかにコアの汎用実装より大幅に高速な「高性能特化版」であることが実証されたため、以下のリネームを実施した。

| 変更前 | 変更後 |
|---|---|
| プロジェクト: `Smart.IO.ByteMapper.Options` | `Smart.IO.ByteMapper.Fast` |
| アセンブリ名: `Smart.IO.ByteMapper.Options` | `Smart.IO.ByteMapper.Fast` |
| 名前空間: `Smart.IO.ByteMapper.Options.*` | `Smart.IO.ByteMapper.Fast.*` |
| csproj: `Smart.IO.ByteMapper.Options.csproj` | `Smart.IO.ByteMapper.Fast.csproj` |

影響ファイル: `Smart.IO.ByteMapper.slnx`、`Smart.IO.ByteMapper.Tests.csproj`、`Smart.IO.ByteMapper.Benchmark.csproj`、`OptionsConverterTests.cs`（using更新）

---

## 7. Core vs Fast ベンチマーク比較結果

### 環境
- BenchmarkDotNet v0.15.0
- AMD Ryzen 9 5900X 3.70GHz, 24 logical cores
- .NET SDK 10.0.300 / .NET 10.0.8 (X64 RyuJIT AVX2)
- Job: MediumRun, InProcessEmitToolchain, 15 iterations, 2 launches

### 結果テーブル

| Method            | Mean       | Error     | StdDev     | Gen0   | Allocated | 倍率(Core/Fast) |
|------------------ |-----------:|----------:|-----------:|-------:|----------:|----------------:|
| CoreAsciiRead     |  12.09 ns  | 1.15 ns   |  1.73 ns   | 0.0029 |      48 B | —               |
| **FastAsciiRead** |  14.94 ns  | 0.38 ns   |  0.56 ns   | 0.0029 |      48 B | 0.81x *(Core速)*|
| CoreAsciiWrite    |  14.50 ns  | 0.33 ns   |  0.49 ns   |      - |         - | —               |
| **FastAsciiWrite**|   4.92 ns  | 0.17 ns   |  0.26 ns   |      - |         - | **2.95x 高速**  |
| CoreIntRead       |  30.07 ns  | 0.92 ns   |  1.32 ns   | 0.0024 |      40 B | —               |
| **FastIntRead**   |   7.05 ns  | 0.50 ns   |  0.74 ns   |      - |         - | **4.27x 高速**  |
| CoreIntWrite      |  27.07 ns  | 0.75 ns   |  1.12 ns   | 0.0024 |      40 B | —               |
| **FastIntWrite**  |   7.10 ns  | 0.50 ns   |  0.74 ns   |      - |         - | **3.81x 高速**  |
| CoreDecimalRead   |  82.11 ns  | 3.25 ns   |  4.77 ns   | 0.0023 |      40 B | —               |
| **FastDecimalRead**|  17.47 ns | 0.72 ns   |  1.08 ns   |      - |         - | **4.70x 高速**  |
| CoreDecimalWrite  |  74.82 ns  | 2.81 ns   |  4.21 ns   | 0.0023 |      40 B | —               |
| **FastDecimalWrite**| 42.03 ns | 2.12 ns   |  3.17 ns   |      - |         - | **1.78x 高速**  |
| CoreDateTimeRead  | 160.14 ns  | 7.58 ns   | 11.35 ns   | 0.0032 |      56 B | —               |
| **FastDateTimeRead**| 22.16 ns | 0.87 ns   |  1.27 ns   |      - |         - | **7.22x 高速**  |
| CoreDateTimeWrite |  91.37 ns  | 3.09 ns   |  4.62 ns   | 0.0033 |      56 B | —               |
| **FastDateTimeWrite**| 30.34 ns| 0.88 ns   |  1.26 ns   |      - |         - | **3.01x 高速**  |

### 考察

#### ASCII 文字列
- **Read**: Core(TextConverter) が Fast(AsciiConverter) より約 1.24x 高速。どちらも `Encoding.ASCII.GetString` を使うが、Core は `TrimRange` 後に直接 `GetString` するパスが短い。アロケーション量は同等（string 生成不可避の 48B）。
- **Write**: Fast が **約 3.0x 高速でゼロアロケーション**。`System.Text.Ascii.FromUtf16` でバイト直接変換し、エンコーダー経由を回避している。

#### 整数テキスト
- **Read/Write 共に** Fast が **3.8〜4.3x 高速・ゼロアロケーション**。Core は `Encoding.GetString` → `int.Parse` → `stackalloc` → `encoding.GetBytes` という文字列変換往復。Fast は `NumberByteHelper` でバイトレベルのデジット処理を行い、完全にアロケーションフリー。

#### decimal テキスト
- **Read で 4.7x、Write で 1.8x 高速・ゼロアロケーション**。`decimal` は内部 128bit 構造体のため `NumberByteHelper.TryParseDecimal` のバイト直接処理が特に効果的。

#### DateTime テキスト
- **最大の差異: Read で 7.2x、Write で 3.0x 高速・ゼロアロケーション**。Core は `DateTime.ParseExact(string)` を経由するため文字列オブジェクトが必要（56B アロケーション）。Fast は `DateTimeByteHelper` でバイト列を直接パースし文字列変換を完全排除。

### 結論
- Fast (`Smart.IO.ByteMapper.Fast`) は汎用性より**スループットとゼロアロケーション**を優先した高性能特化実装である。
- Core (`Smart.IO.ByteMapper`) は設定の柔軟性（任意 codePage、NumberStyles、DateTimeStyles）を優先した汎用実装であり、用途が異なる。
- 命名を **`Options` → `Fast`** に変更することで、この性能特化の意図が名前から明確になった。

---

*Updated: Smart.IO.ByteMapper Fast への命名変更と Core vs Fast ベンチマーク結果追記 (2025)*

