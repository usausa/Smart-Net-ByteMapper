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
}
