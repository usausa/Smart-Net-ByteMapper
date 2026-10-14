namespace Smart.IO.ByteMapper.Generator.Tests;

using Microsoft.CodeAnalysis;

public class DiagnosticTests
{
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

    // ------------------------------------------------------------
    // Method definition (00xx)
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
    // Type (01xx)
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0101TargetNotInstantiatableEmitsDiagnostic()
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
        Assert.Contains(diagnostics, static x => x.Id == "SBM0101");
    }

    [Fact]
    public void Sbm0102BothMapAndMapProfileEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapProfile(4, UseDelimiter = false)]
            public sealed class BothMapRecord
            {
            }

            public static partial class MappersSBM0102
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, BothMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0102");
    }

    [Fact]
    public void Sbm0103ProfileMissingMapAttributeEmitsDiagnostic()
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
        Assert.Contains(diagnostics, static x => x.Id == "SBM0103");
    }

    [Fact]
    public void Sbm0104MissingMapAttributeEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class NoMapRecord { public int Id { get; set; } }

            public static partial class MappersSBM0104
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, NoMapRecord target);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0104");
    }

    // ------------------------------------------------------------
    // Member (02xx)
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0201MemberNameNotFoundInTargetEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0201 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember("Missing", 0, 8)]
            public sealed class ProfileSBM0201
            {
            }

            public static partial class MappersSBM0201
            {
                [ByteWriter(Profile = typeof(ProfileSBM0201))]
                public static partial void Write(Span<byte> buffer, TargetSBM0201 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0201");
    }

    [Fact]
    public void Sbm0203UnsupportedBinaryTypeEmitsDiagnostic()
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
        Assert.Contains(diagnostics, static x => x.Id == "SBM0203");
    }

    [Fact]
    public void Sbm0204ConverterWithStaticWriteEmitsDiagnostic()
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

                public static partial class MappersSBM0204
                {
                    [ByteWriter]
                    public static partial void Write(Span<byte> buffer, StaticWriteRecord source);
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0204");
    }

    [Fact]
    public void Sbm0205PropertyMappingUnderProfileEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0205 { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0205.Code), 0, 8)]
            public sealed class ProfileSBM0205
            {
                [MapText(0, 8)]
                public string Code { get; set; } = default!;
            }

            public static partial class MappersSBM0205
            {
                [ByteWriter(Profile = typeof(ProfileSBM0205))]
                public static partial void Write(Span<byte> buffer, TargetSBM0205 source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0205");
    }

    [Fact]
    public void Sbm0205MemberOnlyProfileEmitsNoDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public sealed class TargetSBM0205b { public string Code { get; set; } = default!; }

            [MapProfile(8, UseDelimiter = false)]
            [MapTextMember(nameof(TargetSBM0205b.Code), 0, 8)]
            public sealed class ProfileSBM0205b
            {
            }

            public static partial class MappersSBM0205b
            {
                [ByteWriter(Profile = typeof(ProfileSBM0205b))]
                public static partial void Write(Span<byte> buffer, TargetSBM0205b source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0205");
    }

    [Fact]
    public void Sbm0206MemberAttributeUnderMapEmitsDiagnostic()
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

            public static partial class MappersSBM0206
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, MemberUnderMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0206");
    }

    [Fact]
    public void Sbm0206PropertyMappingUnderMapEmitsNoDiagnostic()
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

            public static partial class MappersSBM0206b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, PlainMapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0206");
    }

    // ------------------------------------------------------------
    // Layout (03xx)
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0301DelimiterLongerThanMapSizeEmitsDiagnostic()
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

            public static partial class MappersSBM0301
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DelimiterOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0301");
    }

    [Fact]
    public void Sbm0301DelimiterEqualToMapSizeEmitsNoDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(2, Delimiter = new byte[] { 0x0D, 0x0A })]
            public sealed class DelimiterExactRecord
            {
            }

            public static partial class MappersSBM0301b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DelimiterExactRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0301");
    }

    [Fact]
    public void Sbm0301NegativeMemberOffsetEmitsDiagnostic()
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

            public static partial class MappersSBM0301c
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, NegativeOffsetRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0301");
    }

    [Fact]
    public void Sbm0302MemberExceedsMapSizeEmitsDiagnostic()
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

            public static partial class MappersSBM0302a
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0302MemberFitsExactlyEmitsNoDiagnostic()
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

            public static partial class MappersSBM0302b
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ExactRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0302FillerExceedsMapSizeEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapFiller(0, 8)]
            public sealed class FillerOverflowRecord
            {
            }

            public static partial class MappersSBM0302c
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, FillerOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0302ConstantExceedsMapSizeEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            [MapConstant(2, new byte[] { 0x01, 0x02, 0x03 })]
            public sealed class ConstantOverflowRecord
            {
            }

            public static partial class MappersSBM0302d
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, ConstantOverflowRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0302MembersWithGapAllFitWithinMapSizeEmitsNoDiagnostic()
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

            public static partial class MappersSBM0302e
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, GapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0302FastDateTimeExceedsMapSizeEmitsDiagnostic()
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

        Assert.Contains(diagnostics, static d => d.Id == "SBM0302");
    }

    [Fact]
    public void Sbm0303UnknownMemberSizeEmitsDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0303");
    }

    [Fact]
    public void Sbm0303ValidateLayoutFalseEmitsNoDiagnostic()
    {
        const string source = DynamicSizeConverterSource + """

            public static partial class DynamicSizeMappers
            {
                [ByteWriter(ValidateLayout = false)]
                public static partial void Write(Span<byte> buffer, DynamicSizeRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.DoesNotContain(diagnostics, static d => d.Id == "SBM0303");
    }

    [Fact]
    public void Sbm0304OverlappingMembersEmitsDiagnostic()
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

            public static partial class MappersSBM0304
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, OverlapRecord source);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, static d => d.Id == "SBM0304");
    }

    [Fact]
    public void Sbm0304FastDateTimeOverlapEmitsDiagnostic()
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

        Assert.Contains(diagnostics, static d => d.Id == "SBM0304");
    }

    [Fact]
    public void Sbm0304FastDateTimeValidLayoutEmitsNoDiagnostic()
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

        Assert.DoesNotContain(diagnostics, static d => d.Id is "SBM0304" or "SBM0302" or "SBM0303");
    }

    // ------------------------------------------------------------
    // Valid
    // ------------------------------------------------------------

    [Fact]
    public void NullableDateTimeTextGeneratesCompilableCode()
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

    [Fact]
    public void NonNullableBoolReaderGeneratesCompilableCode()
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

    [Fact]
    public void CustomAttributeWithEnumInitializerGeneratesCompilableCode()
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

    [Fact]
    public void CombinedFlagsStyleGeneratesCompilableCode()
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
