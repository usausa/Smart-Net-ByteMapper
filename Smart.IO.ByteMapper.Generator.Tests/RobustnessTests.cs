namespace Smart.IO.ByteMapper.Generator.Tests;

using System.Reflection;

using Microsoft.CodeAnalysis;

public class RobustnessTests
{
    private const string ValidMapper = """

        [Map(4, UseDelimiter = false)]
        public sealed class ValidRecord
        {
            [MapText(0, 4)]
            public string Code { get; set; } = default!;
        }

        public static partial class ValidMappers
        {
            [ByteWriter]
            public static partial void WriteValid(Span<byte> buffer, ValidRecord source);
        }
        """;

    // ------------------------------------------------------------
    // 入力途中の属性で生成器が止まらず、ほかのマッパーは生成される
    // ------------------------------------------------------------

    [Theory]
    [InlineData("[Map]", "[MapText(0, 4)]")]
    [InlineData("[Map(4), MapFiller(0)]", "[MapText(0, 4)]")]
    [InlineData("[Map(4), MapConstant(0)]", "[MapText(0, 4)]")]
    [InlineData("[Map(4)]", "[MapText]")]
    [InlineData("[Map(4)]", "[MapText(0, 4, Filler = 1, Filler = 2)]")]
    public void IncompleteAttributeDoesNotStopOtherMappers(string typeAttributes, string propertyAttributes)
    {
        var source = $$"""
            using System;
            using Smart.IO.ByteMapper;

            {{typeAttributes}}
            public sealed class BrokenRecord
            {
                {{propertyAttributes}}
                public string Code { get; set; } = default!;
            }

            public static partial class BrokenMappers
            {
                [ByteWriter]
                public static partial void WriteBroken(Span<byte> buffer, BrokenRecord source);
            }
            """ + ValidMapper;

        var problems = GeneratorTestHelper.GetProblemIds(source);

        Assert.DoesNotContain("CS8785", problems);
        Assert.Contains("static partial void WriteValid(", GeneratorTestHelper.GetGeneratedText(source), StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // 基底クラスのプロパティも読み書きする
    // ------------------------------------------------------------

    [Fact]
    public void BaseClassPropertiesAreMapped()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public abstract class RecordBase
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            [Map(8, UseDelimiter = false)]
            public sealed class Record : RecordBase
            {
                [MapText(4, 4)]
                public string Name { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);

                [ByteWriter]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        var text = GeneratorTestHelper.GetGeneratedText(source);

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
        Assert.Contains("target.Code = ", text, StringComparison.Ordinal);
        Assert.Contains("source.Code);", text, StringComparison.Ordinal);
    }

    [Fact]
    public void ProfileFindsBaseClassProperty()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            public abstract class RecordBase
            {
                public string Code { get; set; } = default!;
            }

            public sealed class Record : RecordBase
            {
            }

            [MapProfile(4, UseDelimiter = false)]
            [MapTextMember("Code", 0, 4)]
            public sealed class RecordProfile
            {
            }

            public static partial class Mappers
            {
                [ByteWriter(Profile = typeof(RecordProfile))]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // SBM0016 — 生成コードが読み書きできないプロパティ
    // ------------------------------------------------------------

    [Theory]
    [InlineData("public string Code { get; init; } = default!;")]
    [InlineData("public string Code { get; } = default!;")]
    [InlineData("public static string Code { get; set; } = default!;")]
    [InlineData("public string Code { get; private set; } = default!;")]
    public void Sbm0016UnassignablePropertyEmitsDiagnostic(string property)
    {
        var source = $$"""
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                {{property}}
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);
            }
            """;

        Assert.Equal(["SBM0016"], GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void Sbm0011RequiredMemberWithNewInstanceEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public required string Code { get; set; }
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial Record Read(ReadOnlySpan<byte> buffer);
            }
            """;

        Assert.Equal(["SBM0011"], GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // SBM0008 — コンバーターの型がプロパティと合わない
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0008ConverterTypeMismatchEmitsDiagnostic()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapNumberText<int>(0, 4)]
                public long Value { get; set; }
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);
            }
            """;

        Assert.Equal(["SBM0008"], GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // SBM0002 — 値で受け取る構造体への読み込み
    // ------------------------------------------------------------

    [Theory]
    [InlineData("Record target", new[] { "SBM0002" })]
    [InlineData("ref Record target", new string[0])]
    public void StructTargetMustBeReceivedByRef(string parameter, string[] expected)
    {
        // エラーのメソッドにも例外を投げる実装が出るので、CS8795 は並ばない
        var source = $$"""
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public struct Record
            {
                [MapText(0, 4)]
                public string Code { get; set; }
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, {{parameter}});
            }
            """;

        Assert.Equal(expected, GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // 宣言どおりの実装(アクセシビリティ、入れ子、record、キーワードの名前、null 許容)
    // ------------------------------------------------------------

    [Theory]
    [InlineData("static partial void Read(ReadOnlySpan<byte> buffer, Record target);", "static partial class")]
    [InlineData("internal static partial void Read(ReadOnlySpan<byte> buffer, Record target);", "static partial class")]
    [InlineData("public static partial void Read(ReadOnlySpan<byte> buffer, Record target);", "partial record")]
    public void ImplementationRepeatsDeclaration(string method, string container)
    {
        var source = $$"""
            #nullable enable
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public partial class Outer
            {
                public {{container}} Mappers
                {
                    [ByteReader]
                    {{method}}
                }
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void NullableTargetIsNotSupported()
    {
        const string source = """
            #nullable enable
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record? target);
            }
            """;

        Assert.Equal(["SBM0002"], GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void KeywordAndNullablePropertiesCompile()
    {
        const string source = """
            #nullable enable
            using System;
            using Smart.IO.ByteMapper;

            [Map(8, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string @event { get; set; } = default!;

                [MapText(4, 4)]
                public string? Name { get; set; }
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);

                [ByteWriter]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // 診断の文面と抑止
    // ------------------------------------------------------------

    [Fact]
    public void MessageArgumentsMatchFormat()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapBinary<int>(0)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);
            }
            """;

        Assert.Equal(["SBM0007"], GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void WarningCanBeSuppressedAtMethod()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(8, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;

                [MapText(2, 4)]
                public string Name { get; set; } = default!;
            }

            public static partial class Mappers
            {
            #pragma warning disable SBM0005
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);
            #pragma warning restore SBM0005
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // 既定値のないコンバーターの引数は、属性で指定しなければ default を渡す
    // ------------------------------------------------------------

    [Fact]
    public void ConverterParameterWithoutDefaultValueIsGenerated()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapBytes(0, 4)]
                public byte[] Data { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> buffer, Record target);
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void ErrorsCannotBeSuppressed()
    {
        var descriptors = typeof(Diagnostics)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(static x => x.PropertyType == typeof(DiagnosticDescriptor))
            .Select(static x => (DiagnosticDescriptor)x.GetValue(null)!)
            .ToList();

        Assert.All(
            descriptors.Where(static x => x.DefaultSeverity == DiagnosticSeverity.Error),
            static x => Assert.Equal([WellKnownDiagnosticTags.NotConfigurable, WellKnownDiagnosticTags.Compiler], x.CustomTags));
        Assert.All(
            descriptors.Where(static x => x.DefaultSeverity != DiagnosticSeverity.Error),
            static x => Assert.Empty(x.CustomTags));
    }

    // ------------------------------------------------------------
    // SBM0017 : 大文字小文字だけ違う型は最初の 1 つだけ生成する
    // ------------------------------------------------------------

    [Fact]
    public void Sbm0017CaseOnlyTypeNamesGenerateTheFirstOnly()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, Record source);
            }

            public static partial class mappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        var problems = GeneratorTestHelper.GetProblemIds(source);

        Assert.Contains("SBM0017", problems);
        Assert.DoesNotContain("CS8785", problems);
    }

    // ------------------------------------------------------------
    // 生成する名前は利用者の名前と衝突しない
    // ------------------------------------------------------------

    [Fact]
    public void GeneratedNamesDoNotClashWithUserNames()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false, NullFiller = 0x20)]
            [MapConstant(3, new byte[] { 0x0A })]
            public sealed class Record
            {
                [MapText(0, 3)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                private static readonly int Converter0 = 0;

                private static readonly int ConstantBytes0 = 0;

                [ByteReader]
                public static partial Record Read(ReadOnlySpan<byte> target);

                [ByteWriter]
                public static partial byte[] Write(Record buffer);

                [ByteWriter]
                public static partial byte[] WriteSpan(Record span);

                public static int Sum() => Converter0 + ConstantBytes0;
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // 宣言に付けた [MethodImpl] は生成コードで繰り返さない
    // ------------------------------------------------------------

    [Fact]
    public void DeclarationMethodImplIsNotRepeated()
    {
        const string source = """
            using System;
            using System.Runtime.CompilerServices;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteWriter]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // 同じアセンブリの internal の引数なしコンストラクターを使う
    // ------------------------------------------------------------

    [Fact]
    public void InternalParameterlessConstructorIsUsed()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                internal Record()
                {
                }

                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteReader]
                internal static partial Record Read(ReadOnlySpan<byte> source);
            }
            """;

        Assert.Empty(GeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // [Obsolete] のプロパティは警告なしに写像し、エラーのものは SBM0016
    // ------------------------------------------------------------

    [Theory]
    [InlineData("[Obsolete]", new string[0])]
    [InlineData("[Obsolete(\"old\", true)]", new[] { "SBM0016" })]
    public void ObsoletePropertyIsMappedWithoutWarning(string attribute, string[] expected)
    {
        var source = $$"""
            using System;
            using Smart.IO.ByteMapper;

            [Map(4, UseDelimiter = false)]
            public sealed class Record
            {
                {{attribute}}
                [MapText(0, 4)]
                public string Code { get; set; } = default!;
            }

            public static partial class Mappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> buffer, Record source);
            }
            """;

        Assert.Equal(expected, GeneratorTestHelper.GetProblemIds(source));
    }
}
