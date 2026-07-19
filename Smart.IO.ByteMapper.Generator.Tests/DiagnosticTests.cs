namespace Smart.IO.ByteMapper.Generator.Tests;

using Microsoft.CodeAnalysis;

// Source Generator が正しい診断（SBM0001〜SBM0007）を発行することを検証するテスト。
public class DiagnosticTests
{
    // -----------------------------------------------------------------------
    // SBM0001 — メソッドが static partial でない
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0001_NonPartialMethod_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class RecordSBM0001 { [MapBinary<int>(0)] public int Id { get; set; } }

            public static partial class MappersSBM0001
            {
                [ByteReader]
                public static void Read(ReadOnlySpan<byte> buffer, RecordSBM0001 target) { }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0001");
    }

    [Fact]
    public void SBM0001_InstanceMethod_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class RecordSBM0001b { [MapBinary<int>(0)] public int Id { get; set; } }

            public partial class MappersSBM0001b
            {
                [ByteReader]
                public partial void Read(ReadOnlySpan<byte> buffer, RecordSBM0001b target);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0001");
    }

    // -----------------------------------------------------------------------
    // SBM0002 — メソッドシグネチャが不正
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0002_InvalidReaderSignature_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class RecordSBM0002 { [MapBinary<int>(0)] public int Id { get; set; } }

            public static partial class MappersSBM0002
            {
                [ByteReader]
                public static partial void Read(int x, RecordSBM0002 target);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0002");
    }

    // -----------------------------------------------------------------------
    // SBM0003 — ターゲット型に [Map] 属性がない
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0003_MissingMapAttribute_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class NoMapRecord { public int Id { get; set; } }

            public static partial class MappersSBM0003
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, NoMapRecord target);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0003");
    }

    // -----------------------------------------------------------------------
    // SBM0006 — レンジの重複
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0006_OverlappingMembers_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(8, UseDelimiter = false)]
            public sealed class OverlapRecord
            {
                [MapBinary<int>(0)]
                public int A { get; set; }

                [MapBinary<int>(2)]
                public int B { get; set; }
            }

            public static partial class MappersSBM0006
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverlapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    // -----------------------------------------------------------------------
    // SBM0007 — レイアウトが Map(size) を超過
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0007_MemberExceedsMapSize_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class OverflowRecord
            {
                [MapText(0, 10)]
                public string Name { get; set; } = default!;
            }

            public static partial class MappersSBM0007a
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0007");
    }

    [Fact]
    public void SBM0007_MemberFitsExactly_DoesNotEmitDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class ExactRecord
            {
                [MapBinary<int>(0)]
                public int Id { get; set; }
            }

            public static partial class MappersSBM0007b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ExactRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0007");
    }

    [Fact]
    public void SBM0007_FillerExceedsMapSize_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapFiller(0, 8)]
            public sealed class FillerOverflowRecord
            {
            }

            public static partial class MappersSBM0007c
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FillerOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0007");
    }

    [Fact]
    public void SBM0007_ConstantExceedsMapSize_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapConstant(2, new byte[] { 0x01, 0x02, 0x03 })]
            public sealed class ConstantOverflowRecord
            {
            }

            public static partial class MappersSBM0007d
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ConstantOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0007");
    }

    [Fact]
    public void SBM0007_MembersWithGapAllFitWithinMapSize_DoesNotEmitDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(10, UseDelimiter = false)]
            public sealed class GapRecord
            {
                [MapBinary<int>(0)]
                public int Id { get; set; }

                [MapBoolean(8)]
                public bool Flag { get; set; }
            }

            public static partial class MappersSBM0007e
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, GapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0007");
    }

    // -----------------------------------------------------------------------
    // SBM0004 — Delimiter がレコード長を超える（負のオフセット）
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0004_DelimiterLongerThanMapSize_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(2, Delimiter = new byte[] { 0x0D, 0x0A, 0x00, 0x00 })]
            public sealed class DelimiterOverflowRecord
            {
                [MapBinary<short>(0)]
                public short Id { get; set; }
            }

            public static partial class MappersSBM0004
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DelimiterOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0004");
    }

    [Fact]
    public void SBM0004_DelimiterEqualToMapSize_DoesNotEmitDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(2, Delimiter = new byte[] { 0x0D, 0x0A })]
            public sealed class DelimiterExactRecord
            {
            }

            public static partial class MappersSBM0004b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DelimiterExactRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0004");
    }

    [Fact]
    public void SBM0004_NegativeMemberOffset_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(8, UseDelimiter = false)]
            public sealed class NegativeOffsetRecord
            {
                [MapBinary<int>(-4)]
                public int Id { get; set; }
            }

            public static partial class MappersSBM0004c
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, NegativeOffsetRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0004");
    }

    // -----------------------------------------------------------------------
    // SBM0010 — converter の Read / Write が static（インスタンスメソッドでない）
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0010_ConverterWithStaticWrite_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            namespace Custom
            {
                // Read is an instance method, but Write is static — must be rejected.
                public sealed class StaticWriteConverter
                {
                    public const int Size = 4;
                    public int Read(ReadOnlySpan<byte> source) => 0;
                    public static void Write(Span<byte> destination, int value) { }
                }

                public sealed class StaticWriteConverterAttribute : ByteMapperPropertyAttribute<StaticWriteConverter>
                {
                    public StaticWriteConverterAttribute(int offset) : base(offset) { }
                }

                [Map(4, UseDelimiter = false)]
                public sealed class StaticWriteRecord
                {
                    [StaticWriteConverter(0)]
                    public int Id { get; set; }
                }

                public static partial class MappersSBM0010
                {
                    [ByteWriter]
                    public static partial void Write(Span<byte> buffer, StaticWriteRecord source);
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0010");
    }

    // -----------------------------------------------------------------------
    // SBM0015 — [Map] 下でクラスレベルの [Map...Member] 属性が使われている（警告）
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0015_MemberAttributeUnderMap_EmitsWarning()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapBinaryMember<int>(nameof(MemberUnderMapRecord.Id), 0)]
            public sealed class MemberUnderMapRecord
            {
                public int Id { get; set; }
            }

            public static partial class MappersSBM0015
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, MemberUnderMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0015");
    }

    [Fact]
    public void SBM0015_PropertyMappingUnderMap_DoesNotEmitWarning()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class PlainMapRecord
            {
                [MapBinary<int>(0)]
                public int Id { get; set; }
            }

            public static partial class MappersSBM0015b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, PlainMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0015");
    }

    // -----------------------------------------------------------------------
    // SBM0016 — [MapProfile] 下でプロパティに マッピング属性がある（警告）
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0016_PropertyMappingUnderProfile_EmitsWarning()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0016 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0016.Code), 0, 8)]
            public sealed class ProfileSBM0016
            {
                [MapText(0, 8)]
                public string Code { get; set; } = default!;
            }

            public static partial class MappersSBM0016
            {
                [ByteWriter(Profile = typeof(ProfileSBM0016))]
                public static partial void Write(Span<byte> buffer, TargetSBM0016 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0016");
    }

    [Fact]
    public void SBM0016_MemberOnlyProfile_DoesNotEmitWarning()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0016b { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0016b.Code), 0, 8)]
            public sealed class ProfileSBM0016b
            {
            }

            public static partial class MappersSBM0016b
            {
                [ByteWriter(Profile = typeof(ProfileSBM0016b))]
                public static partial void Write(Span<byte> buffer, TargetSBM0016b source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0016");
    }

    // -----------------------------------------------------------------------
    // SBM0017 — [Map] と [MapProfile] の併用（エラー）
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0017_BothMapAndMapProfile_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapProfile(4, UseDelimiter = false)]
            public sealed class BothMapRecord
            {
            }

            public static partial class MappersSBM0017
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, BothMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0017");
    }

    // -----------------------------------------------------------------------
    // SBM0011 — [MapProfile] のメンバー名がターゲットに無い
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0011_MemberNameNotFoundInTarget_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0011 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember("Missing", 0, 8)]
            public sealed class ProfileSBM0011
            {
            }

            public static partial class MappersSBM0011
            {
                [ByteWriter(Profile = typeof(ProfileSBM0011))]
                public static partial void Write(Span<byte> buffer, TargetSBM0011 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0011");
    }

    // -----------------------------------------------------------------------
    // SBM0008 — MapDateTimeText は nullable 型をサポートしない
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0008_NullableDateTimeWithDateTimeText_EmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(8, UseDelimiter = false)]
            public sealed class RecordSBM0008n
            {
                [MapDateTimeText<DateTime>(0, 8, "yyyyMMdd")]
                public DateTime? Date { get; set; }
            }

            public static partial class MappersSBM0008n
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, RecordSBM0008n source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0008");
    }

    // -----------------------------------------------------------------------
    // 非 nullable bool — Reader は .GetValueOrDefault() 付きでコンパイル可能
    // -----------------------------------------------------------------------

    [Fact]
    public void NonNullableBoolReader_GeneratesCompilableCode()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(1, UseDelimiter = false)]
            public sealed class StrictBoolRecord
            {
                [MapBoolean(0)]
                public bool Flag { get; set; }
            }

            public static partial class StrictBoolMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, StrictBoolRecord target);

                [ByteWriter]
                public static partial void Write(Span<byte> buffer, StrictBoolRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        Assert.DoesNotContain(diagnostics, static d => d.Severity == DiagnosticSeverity.Error);
    }

    // -----------------------------------------------------------------------
    // 文字列第1引数(書式)からのサイズ導出 — Fast 日時フィールドもレイアウト検証の対象になる
    // -----------------------------------------------------------------------

    [Fact]
    public void SBM0006_FastDateTimeOverlap_EmitsDiagnostic()
    {
        // "yyyyMMddHHmmss" = 14バイト (0..13) と Qty(8..13) の重複が検出される
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(20, UseDelimiter = false)]
            public sealed class FastOverlapRecord
            {
                [MapFastDateTime(0, "yyyyMMddHHmmss")]
                public DateTime? Timestamp { get; set; }

                [MapFastInteger<int>(8, 6)]
                public int? Qty { get; set; }
            }

            public static partial class FastOverlapMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FastOverlapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void SBM0007_FastDateTimeExceedsMapSize_EmitsDiagnostic()
    {
        // "yyyyMMddHHmmss" = 14バイト > Map(10) が検出される
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(10, UseDelimiter = false)]
            public sealed class FastExceedsRecord
            {
                [MapFastDateTime(0, "yyyyMMddHHmmss")]
                public DateTime? Timestamp { get; set; }
            }

            public static partial class FastExceedsMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FastExceedsRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0007");
    }

    [Fact]
    public void FastDateTimeValidLayout_DoesNotEmitDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(14, UseDelimiter = false)]
            public sealed class FastValidRecord
            {
                [MapFastDateTime(0, "yyyyMMdd")]
                public DateTime? Timestamp { get; set; }

                [MapFastInteger<int>(8, 6)]
                public int? Qty { get; set; }
            }

            public static partial class FastValidMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FastValidRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id is "SBM0006" or "SBM0007" or "SBM0018");
    }

    // -----------------------------------------------------------------------
    // SBM0018 — サイズを静的に導出できないメンバーは検証スキップを警告する
    // -----------------------------------------------------------------------

    private const string DynamicSizeConverterSource = """
        using System;
        using Smart.IO.ByteMapper;

        public sealed class DynamicSizeConverter
        {
            public int Size { get; }

            public DynamicSizeConverter(byte marker)
            {
                Size = 4;
            }

            public int Read(ReadOnlySpan<byte> buffer) => 0;

            public void Write(Span<byte> buffer, int value) { }
        }

        [ConverterSupportedTypes(typeof(int))]
        public sealed class MapDynamicAttribute : ByteMapperPropertyAttribute<DynamicSizeConverter>
        {
            public byte Marker { get; }

            public MapDynamicAttribute(int offset, byte marker)
                : base(offset)
            {
                Marker = marker;
            }
        }

        [Map(8, UseDelimiter = false)]
        public sealed class DynamicSizeRecord
        {
            [MapDynamic(0, 0x01)]
            public int Value { get; set; }
        }
        """;

    [Fact]
    public void SBM0018_UnknownMemberSize_EmitsDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0018");
    }

    [Fact]
    public void SBM0018_ValidateLayoutFalse_DoesNotEmitDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter(ValidateLayout = false)]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0018");
    }

    // -----------------------------------------------------------------------
    // 同一コンパイル内のカスタム属性 — イニシャライザーは定数評価して FQN で出力される
    // -----------------------------------------------------------------------

    [Fact]
    public void CustomAttributeWithEnumInitializer_GeneratesCompilableCode()
    {
        // The attribute class lives in the test compilation, so its property initializers are read
        // from syntax. The raw text "Padding.Left" would not resolve in the generated file (no
        // using directives, global namespace here); it must be emitted fully qualified.
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class CustomTextConverter
            {
                public int Size { get; }
                private readonly Padding padding;
                private readonly byte filler;

                public CustomTextConverter(int length, Padding padding = Padding.Right, byte filler = 0x20)
                {
                    Size = length;
                    this.padding = padding;
                    this.filler = filler;
                }

                public string Read(ReadOnlySpan<byte> buffer) => string.Empty;

                public void Write(Span<byte> buffer, string value) { }
            }

            [ConverterSupportedTypes(typeof(string))]
            public sealed class MapCustomTextAttribute : ByteMapperPropertyAttribute<CustomTextConverter>
            {
                public int Length { get; }

                public Padding Padding { get; init; } = Padding.Left;

                public MapCustomTextAttribute(int offset, int length)
                    : base(offset)
                {
                    Length = length;
                }
            }

            [Map(8, UseDelimiter = false)]
            public sealed class CustomAttrRecord
            {
                [MapCustomText(0, 8)]
                public string Code { get; set; } = default!;
            }

            public static partial class CustomAttrMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, CustomAttrRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        Assert.DoesNotContain(diagnostics, static d => d.Severity == DiagnosticSeverity.Error);
    }

    // -----------------------------------------------------------------------
    // フラグ合成値 — 単一メンバー名を持たない列挙値はキャスト式で出力される
    // -----------------------------------------------------------------------

    [Fact]
    public void CombinedFlagsStyle_GeneratesCompilableCode()
    {
        const string source = """
            using System;
            using System.Globalization;
            using Smart.IO.ByteMapper;

            [Map(10, UseDelimiter = false)]
            public sealed class FlagsStyleRecord
            {
                [MapNumberText<decimal>(0, 10, Style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign)]
                public decimal Price { get; set; }
            }

            public static partial class FlagsStyleMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FlagsStyleRecord source);

                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, FlagsStyleRecord target);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        Assert.DoesNotContain(diagnostics, static d => d.Severity == DiagnosticSeverity.Error);
    }
}
