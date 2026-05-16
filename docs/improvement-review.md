# Smart.IO.ByteMapper 改善検討レポート

対象: `Smart.IO.ByteMapper` / `Smart.IO.ByteMapper.Options` / `Smart.IO.ByteMapper.AspNetCore`
作成日: 2026-05-16
ターゲット: `net8.0` / `net9.0` / `net10.0`

本書は固定長バイトデータと CLR オブジェクト間の高速変換ライブラリ群について、現コードベースの実装を踏まえてパフォーマンス面と機能面の改善余地を整理する。`docs/intrinsics-opportunities.md`、`__パフォーマンス改善候補.md`、`__改善案.md`、`__AOT課題.md` で既出の論点は重複を避けつつ要点のみ再掲する。

---

## 1. 現状把握サマリ

### 1.1 採用済みの最適化 (本レビューでは深掘りしない)

- `IMapConverter.Read/Write` は `ReadOnlySpan<byte>` / `Span<byte>` ベース。
- `BytesHelper.TrimRange` は `IndexOfAnyExcept` / `LastIndexOfAnyExcept` (SIMD 化済 API) を採用済 (`Smart.IO.ByteMapper/IO/ByteMapper/Helpers/BytesHelper.cs:51`)。
- `EncodingByteHelper.FillUnicode` は `MemoryMarshal.Cast<byte,char>().Fill(filler)` 採用済。
- `NumberByteHelper.FormatInt32/Int64` は 2 桁テーブル `Digits2Table` で `% 10` / `/ 10` を半減済 (`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/NumberByteHelper.cs:126,211`)。
- `BinaryConverter` の int/long/short 系は `BinaryPrimitives.*` 経由。
- `BytesHelper.Int64ToDouble` 系は `Unsafe.As<,>` ベース。
- `Int32/Int64/Int16TextConverter` は `Utf8Parser` / `Utf8Formatter` を使用。`[SkipLocalsInit]` 付与済。
- `TypeProfileKeyCache.TryGetValue` は `[SkipLocalsInit]` 付与済。
- `BooleanConverter` / `NullableBooleanConverter` は `static readonly object BoxedTrue/BoxedFalse` キャッシュで boxing なし。
- `BinaryConverter.Decimal` の Write は `Decimal.GetBits(decimal, Span<int>)` + `stackalloc` を使用。
- `DecimalTextConverter` / `DateTimeTextConverter` / `DateTimeOffsetTextConverter` は stackalloc ベースの文字列フォーマットを使用。
- `AsciiConverter.Write` は `Ascii.FromUtf16` で直接書き込み (中間 byte[] なし)。
- `EncodingByteHelper` の Unicode 経路は `MemoryMarshal.Cast` を使用 (`unsafe` コードなし)。
- `Float/Double` テキスト変換 (`FloatTextConverter`, `DoubleTextConverter`) が追加済。
- `TimeSpan` / `DateOnly` / `TimeOnly` テキスト変換が追加済。
- BinaryConverter の Write は `null → filler 全填` で統一済。
- AspNetCore Formatter は `Stream.ReadAsync/WriteAsync(Memory<byte>, CancellationToken)` 対応済。`HttpContext.RequestAborted` を伝播。
- AspNetCore Formatter のリーダー/ライター生成は `ConstructorInfo` キャッシュ経由 (`Activator.CreateInstance` 不使用)。

### 1.2 全体所見

「コアの値型変換層は既にかなりチューニングされている」状態で、残るボトルネックは以下のカテゴリに集約される。

1. **boxing / interface dispatch**: `IMapConverter`/`IMapper` の `object` 経由設計に起因する 24 B / 呼出のアロケーション (README ベンチマーク `ReadIntBinary` 24 B 等が証拠)。
2. **string / byte[] の中間アロケーション**: `DecimalTextConverter` / `DateTimeTextConverter` で残存。
3. **`Decimal.GetBits` の `int[]` 戻り値**: BinaryConverter Write で毎回ヒープ。
4. **AOT 対応欠如**: リフレクション + 動的ジェネリック + IL emit に依存。
5. **機能上の穴**: Float/Double テキスト、Nullable BinaryConverter、Guid、TimeSpan/DateOnly/TimeOnly。
6. **AspNetCore レイヤ**: PipeReader 未使用、CancellationToken 非伝播、`Activator.CreateInstance` でのリーダー/ライター生成。

---

## 2. パフォーマンス改善

### P-1. `IMapConverter` の boxing 解消 (影響度: ★★★★★ / 容易度: 低)

> 「改善案 (大)」は破壊的変更を伴うため容易度: 低。`BooleanConverter` の boxed cache (小・即時) は対応済。

**現状** (`Smart.IO.ByteMapper/IO/ByteMapper/Converters/IMapConverter.cs:3`)

```csharp
public interface IMapConverter
{
    object Read(ReadOnlySpan<byte> buffer);
    void Write(Span<byte> buffer, object value);
}
```

`Read` の戻り値・`Write` の引数が `object` のため、`int`/`long`/`DateTime`/`decimal` などが Read/Write のたびに boxing/unboxing される。README ベンチでも `ReadIntBinary` が 24 B/op を示しており、ホットパスの GC 圧の最大要因。

**改善案 (大)**

```csharp
public interface IMapConverter<T>
{
    T Read(ReadOnlySpan<byte> buffer);
    void Write(Span<byte> buffer, T value);
}

public sealed class MemberMapper<TTarget, TProperty> : IMapper<TTarget>
{
    private readonly int offset;
    private readonly IMapConverter<TProperty> converter;
    private readonly Func<TTarget, TProperty> getter;
    private readonly Action<TTarget, TProperty> setter;

    public void Read(ReadOnlySpan<byte> buffer, TTarget target)
        => setter(target, converter.Read(buffer[offset..]));

    public void Write(Span<byte> buffer, TTarget target)
        => converter.Write(buffer[offset..], getter(target));
}
```

破壊的変更を避けるため、`IMapConverter`(非ジェネリック) を `IMapConverter<object>` のアダプタにして、ジェネリック実装を `where T : ...` で並行提供する漸進アプローチが現実的。

**注意**: `int` の boxing キャッシュは「典型値が分散しないユースケース」(ステータス値、フラグ ID 等) では効くが、一般値では無意味。`bool` 限定での適用が安全で、`BooleanConverter` / `NullableBooleanConverter` では対応済。

### P-2. `MemberMapper` の getter/setter による boxing (影響度: ★★★★★ / 容易度: 低 : P-1 と連動)

`MemberMapper` (`Smart.IO.ByteMapper/IO/ByteMapper/Mappers/MemberMapper.cs:5`) は `Func<object, object>` / `Action<object, object>` を保持する。値型プロパティへのアクセスで P-1 とは独立に boxing が発生する。P-1 のジェネリック化と同時に `delegateFactory.CreateGetter<TTarget, TProperty>(property)` 系の型付きデリゲートを使えば boxing を完全に消せる。

**段階的アプローチ**

1. **Phase A** (非破壊): `IMapConverter` はそのまま、`MemberMapper` だけ `MemberMapper<TTarget>` にする (target だけ型付け)。`TypeMapper<T>` 側で `T target` を渡せるので少なくとも `(object)target` の box は消える。
2. **Phase B**: 値型コンバーター 1 種ずつジェネリック版を導入 (`BooleanConverter` → `IMapConverter<bool>` 等)。`MemberMapper<TTarget, TProperty>` で消費。
3. **Phase C**: 非ジェネリック `IMapConverter` を adapter にして deprecate。

### P-3. `ArrayConverter` の `Array.SetValue/GetValue` boxing (影響度: ★★★★ / 容易度: 低 : P-1 と連動)

`Smart.IO.ByteMapper/IO/ByteMapper/Converters/ArrayConverter.cs:30,50` の `array.SetValue` / `array.GetValue` は要素ごとに boxing/unboxing が発生する。要素数 N の配列で 2N 回の box が走る。

**改善案**:

```csharp
internal sealed class ArrayConverter<T> : IMapConverter<T[]>
{
    private readonly IMapConverter<T> elementConverter;
    ...

    public T[] Read(ReadOnlySpan<byte> buffer)
    {
        var array = new T[length];
        for (var i = 0; i < length; i++)
        {
            array[i] = elementConverter.Read(buffer[(i * elementSize)..]);
        }
        return array;
    }
}
```

P-1 のジェネリック化とセットでないと「内側コンバーター呼び出しでまだ boxing」となるので、P-1 とまとめて検討。

### P-10. `NumberByteHelper.TryParseInt64` 等の SWAR 化

`NumberByteHelper.cs:75` の桁ごとループは 8/18 桁固定長で 8〜18 反復になる。SWAR (8 byte 同時 `'0'` 減算 → 10未満チェック → 桁係数ベクトルとの内積) で実質 2 ブロック化が可能。詳細は `docs/intrinsics-opportunities.md` §4 参照。

ROI が高いのは「18 桁 long を頻繁にパースする」用途。一般用途では P-1〜P-8 を先に潰す方がコスパ良い。

### P-11. `MapperPositionHelper` の組立コスト (影響度: ★ / 容易度: 高)

`MapperFactory` 生成は通常起動時 1 回なので無視可。ただし `Type.GetProperties()` / `GetCustomAttributes()` のリフレクション呼出は AOT・トリミングで効いてくる (§4 参照)。

---

## 3. 機能改善

### F-1. Native AOT 対応 (影響度: ★★★★★ / 容易度: 低)

`__AOT課題.md` で詳細化されているが、要点を再掲:

| 箇所 | 問題 | 対応 |
|------|------|------|
| `MapperFactory.Create(Type)` | `GetMethod` + `MakeGenericMethod` + `Invoke` | ジェネリック API へ集約、`[RequiresDynamicCode]` 明示 |
| `IDelegateFactory` (Smart.Reflection) | Expression / IL Emit | AOT 用代替 (`PropertyInfo.GetValue/SetValue`) または Source Generator |
| `AttributeMappingFactory` | `Type.GetProperties` / `GetCustomAttributes` | `[DynamicallyAccessedMembers(All)]` 付与 |
| `Enum.ToObject(Type, value)` | 動的列挙型 | ジェネリック `EnumConverter<TEnum>` |
| `Activator.CreateInstance` (AspNetCore) | ジェネリックリーダー型生成 | 型ごとの ctor delegate 登録テーブル |

**推奨ロードマップ**

1. **Phase 1** (非破壊): AOT 非対応 API に `[RequiresDynamicCode]` / `[RequiresUnreferencedCode]` を付与しビルド警告で可視化。
2. **Phase 2**: Source Generator (`Smart.IO.ByteMapper.SourceGeneration`) を新設し、`[GeneratedMapping]` 等の属性に基づいてマッピングコードを `partial class` で生成。boxing も同時に消える。
3. **Phase 3**: Source Generator 出力を主推奨 API として README で誘導。リフレクション版は維持 (非 AOT 用途で便利)。

これは P-1〜P-3 のジェネリック化と同根のため、設計上一体で進める価値が大きい。

### F-4. `Guid` 対応

README "Future" に `Guid` サポートが挙がっている。

- Binary: 16 byte 固定。`Guid.TryWriteBytes` / `new Guid(span)` で済む。BigEndian/LittleEndian の選択も必要。
- Text: `"D"`/`"N"`/`"B"`/`"P"`/`"X"` の format string を尊重。`Utf8Parser.TryParse(span, out Guid, ...)` が標準で使える。

### F-6. Source Generator 経路の提供

設計サンプル:

```csharp
[GeneratedMapping(Size = 32)]
[TypeEncoding(932)]
public partial class Data
{
    [MapInteger(0, 8)] public int IntValue { get; set; }
    [MapText(8, 10)]   public string Name { get; set; }
}

// generated:
partial class Data : IByteMappable<Data>
{
    public static void Read(ReadOnlySpan<byte> buffer, Data target) { ... }
    public static void Write(Span<byte> buffer, Data target) { ... }
}
```

利点:
- boxing 完全排除
- リフレクション完全排除 (AOT/trim safe)
- attribute 解析時にコンパイルエラーで検証可能 (現在は実行時例外)

### F-7. `CancellationToken` の伝播 (影響度: ★★ / 容易度: 高)

`AspNetCore` のリーダー/ライターはすべて `CancellationToken` を受け取らない:

```csharp
public ValueTask<object> ReadAsync(Stream stream, long? length); // ← Token なし
```

ASP.NET Core では `HttpContext.RequestAborted` を渡せるべき。同様に `IOutputWriter.WriteAsync` も `Stream.WriteAsync(buffer, token)` で `token` を伝搬。

### F-8. AspNetCore の non-MVC 経路サポート (影響度: ★ / 容易度: 中)

現状 MVC `InputFormatter`/`OutputFormatter` のみ。Minimal API (`MapPost`/`MapGet`) で使えるバインダ (`IBindAsync` 実装、または `IEndpointFilter` ベースの DI) を提供すると現代的なシナリオに合う。

### F-9. `ReadOnlySequence<byte>` のサポート (影響度: ★★ / 容易度: 中)

`IMapConverter.Read(ReadOnlySpan<byte> buffer)` は線形バッファ前提。`PipeReader` から来る `ReadOnlySequence<byte>` は連続 Span に正規化が必要。

- `ITypeMapper.FromByte(ReadOnlySequence<byte>, T target)` オーバーロード追加。
- 内部で `sequence.IsSingleSegment` なら `FirstSpan`、そうでなければスタック領域へ `CopyTo` (mapper.Size は固定で既知)。

これにより F-7 / P-12 と組み合わせて `PipeReader` パスがクリーンに書ける。

### F-10. 入力検証の強化 (影響度: ★ / 容易度: 高)

- `MapperFactory.Create<T>()` 未登録時の例外メッセージは `type=[...]` 程度。利用可能な型一覧を `Diagnostics` 側だけでなく例外メッセージに含めると DX が向上。
- `AbstractMapConverterBuilder` の `Entries[type]` は未登録時 `KeyNotFoundException`。`Match(type)` を必ず先に呼ぶ規約だが、例外型を `ByteMapperException` で揃えると `try/catch` が一本化できる。

### F-11. ベンチ・テストの拡充 (影響度: ★ / 容易度: 高)

- `Smart.IO.ByteMapper.Benchmark/ConverterBenchmark.cs` は read/write の単体測定。`MemberMapper` 経由 (boxing 含む) と pure converter (boxing 除く) の比較ベンチを追加すると、P-1 系改善前後の効果が一目で出る。
- `IntrinsicsBenchmark.cs` は SIMD 検証用に新設済 (intrinsics-opportunities.md と対応)。Decimal pass 用の `WriteDecimal28Max` 等は `ConverterBenchmark.cs` 側にある。

---

## 4. 優先度マトリクス

容易度の凡例: **高** = 1〜数ファイルの局所改修・API 互換維持。**中** = 複数箇所改修・テスト/ベンチ追加要・小さな仕様詰めあり。**低** = 設計変更や破壊的変更、または高度な実装技法 (SWAR/SourceGenerator/AOT 対応) を要する。

| # | 項目 | 影響 | 容易度 | 既存ドキュメント |
|---|------|------|--------|------------------|
| **P-1 (大)** | `IMapConverter<T>` ジェネリック化 | ★★★★★ | 低 | __改善案.md §8 |
| **P-2** | `MemberMapper` のジェネリック化 | ★★★★★ | 低 | __改善案.md §8 |
| **P-3** | `ArrayConverter` のジェネリック化 | ★★★★ | 低 | __改善案.md §8 |
| **P-10** | `NumberByteHelper.TryParse` SWAR 化 | ★★★ | 低 | intrinsics-opportunities.md §4 |
| **P-11** | `MapperPositionHelper` 組立コスト | ★ | 高 | 本書新規 |
| **F-1** | Native AOT 対応 | ★★★★★ | 低 | __AOT課題.md 全般 |
| **F-4** | Guid サポート | ★★ | 高 | README Future |
| **F-6** | Source Generator (F-1 内包) | ★★★★ | 低 | 本書新規 |
| **F-8** | AspNetCore non-MVC 経路サポート | ★ | 中 | 本書新規 |
| **F-9** | `ReadOnlySequence<byte>` 対応 | ★★ | 中 | 本書新規 |
| **F-10** | 入力検証・例外メッセージ強化 | ★ | 高 | 本書新規 |
| **F-11** | ベンチ・テスト拡充 | ★ | 高 | 本書新規 |

---

## 5. 推奨着手順

短期 (1〜2 スプリント):

1. **P-11** : `MapperPositionHelper` 組立コストの確認。
2. **F-4** : `Guid` サポート追加。API 拡張のみで破壊的でない。

中期 (1〜2 リリース):

3. **F-8 + F-9** : AspNetCore `PipeReader` 経路 + `ReadOnlySequence<byte>` 対応。

長期 (アーキテクチャ変更):

4. **P-1 (大) + P-2 + P-3** : `IMapConverter<T>` 化 + `MemberMapper<TTarget, TProperty>` 化。boxing 撲滅。
5. **F-1 + F-6** : AOT 対応と Source Generator 提供。`Smart.IO.ByteMapper.SourceGeneration` パッケージ新設。

---

## 6. 参考: 既存ベンチマーク数値 (README 抜粋)

`ReadIntBinary` 2.7 ns / 24 B  →  P-1(大) 適用後 ~0 B 期待
`ReadBoolean` 2.7 ns / 24 B    →  **対応済 (boxed cache) → 0 B**
`ReadDecimal28Max` 36 ns / 32 B → **対応済 (stackalloc) → さらに削減**
`ReadAscii13Code` 8.6 ns / 48 B → string 自体は不可避だが内部追加アロケーションは除去済
`WriteAscii13Code` 10.9 ns / 40 B → **対応済 (直接書込) → ~0 B 期待**
`WriteDecimal28Max` 117 ns / 32 B → **対応済 (stackalloc) → 削減済**

これらは AMD Ryzen 9 5900X / .NET 10 / MediumRun 計測値。本書の改善案を順次適用してベンチ回帰を確認することを推奨。
