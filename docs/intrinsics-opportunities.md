# Intrinsics 適用候補の調査メモ

`Smart.IO.ByteMapper` / `Smart.IO.ByteMapper.Options` のホットパス (主に `IMapConverter.Read` / `Write` から呼ばれる箇所) を対象に、`System.Runtime.Intrinsics` (Sse2/Ssse3/Avx2/Avx512、ARM の AdvSimd) や `Vector128/256/512<T>`、SWAR (SIMD Within A Register) 等で高速化できそうな箇所を洗い出した。

ターゲットは `net8.0;net9.0;net8.0` なので、`Vector128<T>.LoadUnsafe`、`Vector256<T>.Equals`、`ExtractMostSignificantBits`、`Vector128.Narrow / Widen` 等は全 TFM で利用可能。クロスプラットフォーム性は `Vector128<T>` ベースに `IsHardwareAccelerated` フォールバックで担保するのが基本方針。

## 評価軸

- **影響度** : ホットパスでの呼出頻度 × 1 回あたりの削減量。
- **難度**   : 仕様 (パディング・トリム・filler・グルーピング等) を維持したまま SIMD 化する難しさ。
- **方針**   : 採用すべき技法 (SWAR / SIMD) の概略。

凡例: 影響度 ★★★ (大) / ★★ (中) / ★ (小)。

---

## 1. `BytesHelper.TrimRange` ★★★

`Smart.IO.ByteMapper/IO/ByteMapper/Helpers/BytesHelper.cs:51`

```csharp
if (padding == Padding.Left)
{
    var end = start + size;
    while ((start < end) && (buffer[start] == filler))
    {
        start++;
        size--;
    }
}
else
{
    while ((size > 0) && (buffer[start + size - 1] == filler))
    {
        size--;
    }
}
```

- `TextConverter.Read` / `AsciiConverter.Read` / 各 `*TextConverter.Read` から都度呼ばれる、ライブラリ全体で最も呼ばれるトリム関数。
- 1 byte ずつの線形走査。例えば `Length=20` で全部 filler のケースだと 20 回ループ。

**方針**: `Vector128<byte>` / `Vector256<byte>` で `filler` を全レーンに展開し、`Vector.Equals` → `ExtractMostSignificantBits` → `TrailingZeroCount`(左) / `LeadingZeroCount`(右) で「最初に filler 以外が出現する位置」を 16 / 32 byte 単位で算出する。`MemoryExtensions.IndexOfAnyExcept(filler)` がほぼ同等の処理を SIMD 実装しているので、まずはそちらで十分か検証してから自前実装を検討すべき。

**注意**: API の `ref int start, ref int size` を維持する必要があるため、内部で `IndexOfAnyExcept` 系に置き換えるだけで足りる。

---

## 2. `EncodingByteHelper.GetAsciiBytes` / `GetAsciiString` ★★★

`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/EncodingByteHelper.cs:12, 35`

```csharp
for (var i = 0; i < length; i++)
{
    *pd = (byte)*ps;   // char → byte
    ps++; pd++;
}
```

- 1 文字ずつの char→byte / byte→char 変換。`AsciiConverter.Read/Write`、および Sjis/Unicode 系を経由するパスでも繰り返し呼ばれる。

**方針**:
- `char → byte` (Narrow): `Vector128.LoadUnsafe(ref Unsafe.As<char,ushort>(ref src))` を 2 ベクトル読み、`Vector128.Narrow(lo, hi)` で 16 文字 → 16 byte に。32 文字単位で処理し、残余をスカラ。
- `byte → char` (Widen): `Vector128.Widen(Vector128.LoadUnsafe(...))` で 16 byte → `(Vector128<ushort>, Vector128<ushort>)`。
- いずれも標準ライブラリの `Ascii.FromUtf16` / `Ascii.ToUtf16` (`System.Text.Ascii`、.NET 8+) が同等の SIMD 実装を内部で行っているので置き換えだけで完結する可能性が高い。

**注意**: 非 ASCII 文字が混入したときの挙動は現状 `(byte)*ps` で下位 8bit を切るだけなので、`Ascii.From/ToUtf16` の戻り値 (`OperationStatus`) を無視するモードを使う必要あり。妥当な代替として `MemoryMarshal.Cast<char,ushort>()` + 手書き SIMD でもよい。

---

## 3. `EncodingByteHelper.FillUnicode` / Unicode トリム ★★

`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/EncodingByteHelper.cs:61, 146`

```csharp
// FillUnicode
for (var i = 0; i < bytes.Length; i += 2)
{
    bytes[i]     = filler1;
    bytes[i + 1] = filler2;
}

// Unicode トリム (Read 側)
while ((index + 1 < end) && (buffer[index] == filler1) && (buffer[index + 1] == filler2))
{
    index += 2;
    length -= 2;
}
```

**方針**:
- `FillUnicode`: `MemoryMarshal.Cast<byte,char>(span).Fill(filler)` 一発で済む。`Span<char>.Fill` は内部で SIMD 化済み。
- Unicode トリム: 同じく `Cast<byte,ushort>` してから `IndexOfAnyExcept(filler)` で左/右どちらでも線形走査をベクトル化できる。

**注意**: 元バッファの開始 index と長さがいずれも偶数前提 (UTF-16 LE) なので `Cast<>` は安全。

---

## 4. `NumberByteHelper.TryParseInt32 / TryParseInt64` ★★★

`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/NumberByteHelper.cs:53, 61`

```csharp
while (i < length)
{
    var num = *(pBytes + i) - Num0;
    if (IsValidNumber(num))
    {
        value = (value * 10) + num;
        i++;
    }
    else { ... break; }
}
```

- 桁数 18 の long の場合 18 回、Decimal の場合 28〜29 回ループ。`*TextConverter` ではなく `IntegerConverter` (Options) のホットパス。

**方針 (SWAR)**: `Utf8Parser` 相当の処理を内製する場合、`ulong` に 8 byte ロード → `'0'` のベクタ減算 → 全レーン `< 10` チェック (`Vector128.LessThan(v, Vector128.Create((byte)10))` の `MoveMask`) → 各レーンの数値を `1, 10, 100, ...` の係数ベクトルで畳み込み。8 桁分を 1 命令ブロックで処理できる。typical な 8/18 桁固定長は 2 ブロックで完結。

**代替案**: 既に `Smart.IO.ByteMapper` 本体の `Int32TextConverter` 等は `Utf8Parser.TryParse` を使っており十分高速。`NumberByteHelper` 側はトリム/filler 仕様 (連続 filler の扱い、空白/数字/filler の混在) が独自なので、`Utf8Parser` への単純置換はできない。SWAR 化するなら「先頭/末尾 filler を SIMD でスキップしてから純粋数字部分だけ Utf8Parser に流す」というハイブリッドが投資対効果が高い。

---

## 5. `NumberByteHelper.FormatInt32 / FormatInt64` ★★

`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/NumberByteHelper.cs:112, 187`

```csharp
while (i >= 0)
{
    *(pBytes + i--) = (byte)(Num0 + (value % 10));
    value /= 10;
    if (value == 0) break;
}
```

- 1 桁ずつ `% 10` / `/ 10`。除算は遅い。

**方針**:
- 2 桁同時生成 (`% 100` → 2 桁テーブル lookup) は同プロジェクト内 `DateTimeByteHelper.FormatDateTimePart2` がすでに採用 (`Table[value]` で 2 byte ペア)。同じ手を `FormatInt32/Int64` に移植するだけで実除算回数が半減する。
- 8 桁/16 桁固定の高速化には Lemire / "itoa, faster" 系の SWAR (`x86_64` 環境なら `Bmi2.Pdep` も使える) があるが、可変長 + パディング + ゼロフィルの仕様があるため複雑度が高い。先に 2 桁テーブル化で測定するのが妥当。

---

## 6. `NumberByteHelper.TryParseDecimal` / `FormatDecimal` ★★

`Smart.IO.ByteMapper.Options/IO/ByteMapper/Helpers/NumberByteHelper.cs:343, 462`

- パース側は §4 と同じ理由で 1 byte ずつ。28〜29 桁あるので影響は無視できない。
- フォーマット側はマンチサ抽出 (`AddBitBlockValue`) と桁ごとの `% 10` / `/ 10` ループ。前者は `Pmullw` / `Pmaddwd` 系での 16bit 並列加算が候補だが、`Decimal` の構造上ヘビーな書換えが必要。

**方針**: §4 と同様、まず先頭/末尾 filler スキャンを SIMD 化、桁部分を SWAR 化する漸進アプローチ。ROI を見極めるためベンチに `WriteDecimal28Max` / `ReadDecimal28Max` が既にあるので比較しやすい。

---

## 7. `BigEndianDecimalBinaryConverter` / `LittleEndianDecimalBinaryConverter` ★

`Smart.IO.ByteMapper/IO/ByteMapper/Converters/BinaryConverter.cs:141`

```csharp
var flag = BinaryPrimitives.ReadInt32BigEndian(span);
var hi   = BinaryPrimitives.ReadInt32BigEndian(span[4..]);
var mid  = BinaryPrimitives.ReadInt32BigEndian(span[8..]);
var lo   = BinaryPrimitives.ReadInt32BigEndian(span[12..]);
```

- 16 byte を 4 回に分けて `BSWAP`。

**方針**: `Vector128<byte>.LoadUnsafe` → `Ssse3.Shuffle` (or `Vector128.Shuffle`) で 4×4 のエンディアン反転を 1 命令で行い、続けて `Vector128.AsInt32()` で取り出す。`Decimal.GetBits` 側も同様にベクトル化可能。
- 単発呼び出しが多いため 1 件あたりの利益は小さいが、`Decimal` の配列マッピング時は効く。

---

## 8. ホットパス全般のオブジェクト経路 (参考)

`MemberMapper.Read/Write` → `IMapConverter.Read/Write(object)` の `box`/`unbox` は SIMD と直接関係しないが、以下の修正と Intrinsics 化の組み合わせで効果が出やすい:

- `IMapConverter` をジェネリックに分けて `box` を回避する案 (別チケット推奨)。
- `ArrayConverter` の `array.SetValue/GetValue` も同様に boxing 経由でホット (`Smart.IO.ByteMapper/IO/ByteMapper/Converters/ArrayConverter.cs:30, 50`)。

---

## 推奨着手順

1. **§1 `BytesHelper.TrimRange`** : 標準 API (`IndexOfAnyExcept`) で書き換え。テスト/ベンチ既存。最速で効果が出る。
2. **§2 `EncodingByteHelper` ASCII** : `System.Text.Ascii` 一本化。コード削減・SIMD 化を同時に達成。
3. **§3 `EncodingByteHelper` Unicode (Fill/Trim)** : `MemoryMarshal.Cast` 経由で `Span<char>.Fill` / `IndexOfAnyExcept`。
4. **§5 `FormatInt32/Int64`** : 2 桁テーブル化 (`DateTimeByteHelper` のパターン流用)。
5. **§4 / §6 数値パース・Decimal フォーマット** : SWAR + ハイブリッド方針で個別 PR。
6. **§7 `Decimal` バイナリ変換** : `Vector128.Shuffle` でエンディアン反転。

各項目は `ConverterBenchmark` に対応するベンチが揃っているため、PR 単位で before/after が明確に出る。

---

## 補足: 未使用ファイル

`Smart.IO.ByteMapper.Benchmark/Utf8NumberTextConverter.cs` および `Smart.IO.ByteMapper.Benchmark/Utf8ConverterBenchmark.cs` は現在空ファイル。`NumberTextConverter` (`Utf8Parser/Utf8Formatter` ベース) と `IntegerConverter` (`NumberByteHelper` ベース) の比較ベンチを置く場所として準備されている可能性が高く、上記 §4〜§5 の改善検証に活用できる。
