namespace Smart.IO.ByteMapper.Generator.Tests;

using Microsoft.CodeAnalysis;

public class DiagnosticTest
{
    // ------------------------------------------------------------
    // Binary type
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0007UnsupportedBinaryTypeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using System;
            using Smart.IO.ByteMapper;

            namespace Test;

            [Map(36, UseDelimiter = false)]
            public sealed class SampleRecord
            {
                [MapBinary<int>(0)]
                public string Name { get; set; } = default!;
            }

            public static partial class SampleMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, SampleRecord target);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "SBM0007");
    }

    // ------------------------------------------------------------
    // Profile
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0010ProfileMissingMapAttributeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using System;
            using Smart.IO.ByteMapper;

            namespace Test;

            [Map(36, UseDelimiter = false)]
            public sealed class SampleRecord
            {
                [MapText(0, 32)]
                public string Name { get; set; } = default!;
            }

            public sealed class SampleProfile
            {
            }

            public static partial class SampleMappers
            {
                [ByteReader(Profile = typeof(SampleProfile))]
                public static partial void Read(ReadOnlySpan<byte> buffer, SampleRecord target);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "SBM0010");
    }

    // ------------------------------------------------------------
    // Target instantiation
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0011TargetNotInstantiatableEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using System;
            using Smart.IO.ByteMapper;

            namespace Test;

            [Map(36, UseDelimiter = false)]
            public sealed class SampleRecord
            {
                public SampleRecord(int id)
                {
                    Id = id;
                }

                [MapBinary<int>(0)]
                public int Id { get; set; }
            }

            public static partial class SampleMappers
            {
                [ByteReader]
                public static partial SampleRecord Read(ReadOnlySpan<byte> buffer);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "SBM0011");
    }

    // ------------------------------------------------------------
    // SBM0001 — メソッドが static partial でない
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0001NonPartialMethodEmitsDiagnostic()
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
    public void Sbm0001InstanceMethodEmitsDiagnostic()
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

    // ------------------------------------------------------------
    // SBM0002 — メソッドシグネチャが不正
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0002InvalidReaderSignatureEmitsDiagnostic()
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

    // ------------------------------------------------------------
    // SBM0003 — ターゲット型に [Map] 属性がない
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0003MissingMapAttributeEmitsDiagnostic()
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

    // ------------------------------------------------------------
    // SBM0005 — レンジの重複
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0005OverlappingMembersEmitsDiagnostic()
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

            public static partial class MappersSBM0005
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverlapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0005");
    }

    // ------------------------------------------------------------
    // SBM0006 — レイアウトが Map(size) を超過
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0006MemberExceedsMapSizeEmitsDiagnostic()
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

            public static partial class MappersSBM0006a
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void Sbm0006MemberFitsExactlyEmitsNoDiagnostic()
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

            public static partial class MappersSBM0006b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ExactRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void Sbm0006FillerExceedsMapSizeEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapFiller(0, 8)]
            public sealed class FillerOverflowRecord
            {
            }

            public static partial class MappersSBM0006c
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FillerOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void Sbm0006ConstantExceedsMapSizeEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapConstant(2, new byte[] { 0x01, 0x02, 0x03 })]
            public sealed class ConstantOverflowRecord
            {
            }

            public static partial class MappersSBM0006d
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ConstantOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void Sbm0006MembersWithGapAllFitWithinMapSizeEmitsNoDiagnostic()
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

            public static partial class MappersSBM0006e
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, GapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0006");
    }

    // ------------------------------------------------------------
    // SBM0004 — Delimiter がレコード長を超える（負のオフセット）
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0004DelimiterLongerThanMapSizeEmitsDiagnostic()
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
    public void Sbm0004DelimiterEqualToMapSizeEmitsNoDiagnostic()
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
    public void Sbm0004NegativeMemberOffsetEmitsDiagnostic()
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

    // ------------------------------------------------------------
    // SBM0008 — converter の Read / Write が static（インスタンスメソッドでない）
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0008ConverterWithStaticWriteEmitsDiagnostic()
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

                public static partial class MappersSBM0008
                {
                    [ByteWriter]
                    public static partial void Write(Span<byte> buffer, StaticWriteRecord source);
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0008");
    }

    // ------------------------------------------------------------
    // SBM0012 — [Map] 下でクラスレベルの [Map...Member] 属性が使われている（警告）
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0012MemberAttributeUnderMapEmitsDiagnostic()
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

            public static partial class MappersSBM0012
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, MemberUnderMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0012");
    }

    [Fact]
    public void Sbm0012PropertyMappingUnderMapEmitsNoDiagnostic()
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

            public static partial class MappersSBM0012b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, PlainMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0012");
    }

    // ------------------------------------------------------------
    // SBM0013 — [MapProfile] 下でプロパティに マッピング属性がある（警告）
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0013PropertyMappingUnderProfileEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0013 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0013.Code), 0, 8)]
            public sealed class ProfileSBM0013
            {
                [MapText(0, 8)]
                public string Code { get; set; } = default!;
            }

            public static partial class MappersSBM0013
            {
                [ByteWriter(Profile = typeof(ProfileSBM0013))]
                public static partial void Write(Span<byte> buffer, TargetSBM0013 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0013");
    }

    [Fact]
    public void Sbm0013MemberOnlyProfileEmitsNoDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0013b { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0013b.Code), 0, 8)]
            public sealed class ProfileSBM0013b
            {
            }

            public static partial class MappersSBM0013b
            {
                [ByteWriter(Profile = typeof(ProfileSBM0013b))]
                public static partial void Write(Span<byte> buffer, TargetSBM0013b source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0013");
    }

    // ------------------------------------------------------------
    // SBM0014 — [Map] と [MapProfile] の併用（エラー）
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0014BothMapAndMapProfileEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapProfile(4, UseDelimiter = false)]
            public sealed class BothMapRecord
            {
            }

            public static partial class MappersSBM0014
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, BothMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0014");
    }

    // ------------------------------------------------------------
    // SBM0009 — [MapProfile] のメンバー名がターゲットに無い
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0009MemberNameNotFoundInTargetEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0009 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember("Missing", 0, 8)]
            public sealed class ProfileSBM0009
            {
            }

            public static partial class MappersSBM0009
            {
                [ByteWriter(Profile = typeof(ProfileSBM0009))]
                public static partial void Write(Span<byte> buffer, TargetSBM0009 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0009");
    }

    // ------------------------------------------------------------
    // nullable 日時 — null ⇔ 全フィラー対応により Reader/Writer ともコンパイル可能
    // ------------------------------------------------------------

    [Fact]
    public void NullableDateTimeText_GeneratesCompilableCode()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(16, UseDelimiter = false)]
            public sealed class NullableDateRecord
            {
                [MapDateTimeText<DateTime>(0, 8, "yyyyMMdd")]
                public DateTime? Date { get; set; }

                [MapDateTimeText<DateOnly>(8, 8, "yyyyMMdd")]
                public DateOnly? DateOnlyValue { get; set; }
            }

            public static partial class NullableDateMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, NullableDateRecord target);

                [ByteWriter]
                public static partial void Write(Span<byte> buffer, NullableDateRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        Assert.DoesNotContain(diagnostics, static d => d.Severity == DiagnosticSeverity.Error);
    }

    // ------------------------------------------------------------
    // 非 nullable bool — Reader は .GetValueOrDefault() 付きでコンパイル可能
    // ------------------------------------------------------------

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

    // ------------------------------------------------------------
    // 文字列第1引数(書式)からのサイズ導出 — Fast 日時フィールドもレイアウト検証の対象になる
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0005FastDateTimeOverlapEmitsDiagnostic()
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

        Assert.Contains(diagnostics, static d => d.Id == "SBM0005");
    }

    [Fact]
    public void Sbm0006FastDateTimeExceedsMapSizeEmitsDiagnostic()
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

        Assert.Contains(diagnostics, static d => d.Id == "SBM0006");
    }

    [Fact]
    public void Sbm0005FastDateTimeValidLayoutEmitsNoDiagnostic()
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

        Assert.DoesNotContain(diagnostics, static d => d.Id is "SBM0005" or "SBM0006" or "SBM0015");
    }

    // ------------------------------------------------------------
    // SBM0015 — サイズを静的に導出できないメンバーは検証スキップを警告する
    // ------------------------------------------------------------

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
    public void Sbm0015UnknownMemberSizeEmitsDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0015");
    }

    [Fact]
    public void Sbm0015ValidateLayoutFalseEmitsNoDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter(ValidateLayout = false)]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0015");
    }

    // ------------------------------------------------------------
    // 同一コンパイル内のカスタム属性 — イニシャライザーは定数評価して FQN で出力される
    // ------------------------------------------------------------

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

    // ------------------------------------------------------------
    // フラグ合成値 — 単一メンバー名を持たない列挙値はキャスト式で出力される
    // ------------------------------------------------------------

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
