namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using System.Reflection;

using Microsoft.CodeAnalysis;

public class DiagnosticTests
{
    private const string Entity = """
        using System;
        using Smart.IO.ByteMapper;
        using Smart.IO.ByteMapper.AspNetCore;

        namespace Test;

        [Map(33)]
        public sealed class SampleData
        {
            [MapText(0, 13)]
            public string Code { get; set; } = default!;

            [MapText(13, 20)]
            public string Name { get; set; } = default!;
        }

        public sealed class Unmapped
        {
            public string Code { get; set; } = default!;
        }
        """;

    // ------------------------------------------------------------
    // SBM1001 : a reader has no matching writer
    // ------------------------------------------------------------

    [Fact]
    public void Sbm1001ReaderWithoutWriterEmitsDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);
            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "SBM1001");
    }

    // ------------------------------------------------------------
    // SBM1002 : the entity size cannot be resolved
    // ------------------------------------------------------------

    [Fact]
    public void Sbm1002UnknownEntitySizeEmitsDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class UnmappedMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, Unmapped target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, Unmapped source);
            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "SBM1002");
    }

    // ------------------------------------------------------------
    // A complete pair must stay clean
    // ------------------------------------------------------------

    [Fact]
    public void ValidEndPointEmitsNoDiagnostic()
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """);

        Assert.Empty(diagnostics);
    }

    // ------------------------------------------------------------
    // SBM1003 : a writer has no reader to pair
    // ------------------------------------------------------------

    [Fact]
    public void Sbm1003WriterWithoutReaderEmitsDiagnostic()
    {
        var problems = AspNetCoreGeneratorTestHelper.GetProblemIds(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """);

        Assert.Equal(["SBM1003"], problems);
    }

    [Fact]
    public void Sbm1003ReaderReturningNewInstanceIsNotPaired()
    {
        var problems = AspNetCoreGeneratorTestHelper.GetProblemIds(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial SampleData Read(ReadOnlySpan<byte> source);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """);

        Assert.Equal(["SBM1003"], problems);
    }

    // ------------------------------------------------------------
    // Generated bindings compile
    // ------------------------------------------------------------

    [Fact]
    public void WriterReturningArrayIsAdapted()
    {
        var source = Entity +
            """

            [ByteMapperEndpoint]
            public static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial byte[] Write(SampleData source);
            }
            """;

        Assert.Empty(AspNetCoreGeneratorTestHelper.GetProblemIds(source));
        Assert.Contains(AspNetCoreGeneratorTestHelper.GetGeneratedSources(source), static x => x.Contains("global::System.MemoryExtensions.CopyTo(Write(s), d)", StringComparison.Ordinal));
    }

    [Fact]
    public void SameEntityNamesInOtherNamespacesDoNotCollide()
    {
        const string source = """
            using System;
            using Smart.IO.ByteMapper;
            using Smart.IO.ByteMapper.AspNetCore;

            namespace V1
            {
                [Map(4)]
                public sealed class Item
                {
                    [MapText(0, 2)]
                    public string Code { get; set; } = default!;
                }
            }

            namespace V2
            {
                [Map(4)]
                public sealed class Item
                {
                    [MapText(0, 2)]
                    public string Code { get; set; } = default!;
                }
            }

            namespace Test
            {
                [ByteMapperEndpoint]
                public static partial class ItemMappers
                {
                    [ByteReader]
                    public static partial void Read1(ReadOnlySpan<byte> source, V1.Item target);

                    [ByteWriter]
                    public static partial void Write1(Span<byte> destination, V1.Item source);

                    [ByteReader]
                    public static partial void Read2(ReadOnlySpan<byte> source, V2.Item target);

                    [ByteWriter]
                    public static partial void Write2(Span<byte> destination, V2.Item source);
                }
            }
            """;

        Assert.Empty(AspNetCoreGeneratorTestHelper.GetProblemIds(source));
    }

    [Fact]
    public void InternalEntityGetsInternalFactory()
    {
        var source = Entity.Replace("public sealed class SampleData", "internal sealed class SampleData", StringComparison.Ordinal) +
            """

            [ByteMapperEndpoint]
            internal static partial class SampleDataMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """;

        Assert.Empty(AspNetCoreGeneratorTestHelper.GetProblemIds(source));
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
    }

    // ------------------------------------------------------------
    // Nested endpoint classes repeat the containing types
    // ------------------------------------------------------------

    [Fact]
    public void NestedEndpointClassRepeatsContainingTypes()
    {
        var source = Entity +
            """

            public static partial class Outer
            {
                [ByteMapperEndpoint]
                public static partial class SampleDataMappers
                {
                    [ByteReader]
                    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                    [ByteWriter]
                    public static partial void Write(Span<byte> destination, SampleData source);
                }
            }
            """;

        Assert.Empty(AspNetCoreGeneratorTestHelper.GetProblemIds(source));
        Assert.Contains(AspNetCoreGeneratorTestHelper.GetGeneratedSources(source), static x => x.Contains("global::Test.Outer.SampleDataMappers.CreateByteMapperBinding()", StringComparison.Ordinal));
    }

    // ------------------------------------------------------------
    // SBM1004 : the endpoint class cannot be extended by the generated code
    // ------------------------------------------------------------

    [Theory]
    [InlineData("public static class Outer", "public static partial class SampleDataMappers")]
    [InlineData("public static partial class Outer", "private static partial class SampleDataMappers")]
    [InlineData("public partial class Outer<T>", "public static partial class SampleDataMappers")]
    [InlineData("public static partial class Outer", "public static class SampleDataMappers")]
    public void Sbm1004InvalidEndpointClassEmitsDiagnostic(string outer, string inner)
    {
        var diagnostics = AspNetCoreGeneratorTestHelper.GetDiagnostics(Entity +
            $$"""

            {{outer}}
            {
                [ByteMapperEndpoint]
                {{inner}}
                {
                    [ByteReader]
                    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                    [ByteWriter]
                    public static partial void Write(Span<byte> destination, SampleData source);
                }
            }
            """);

        Assert.Contains(diagnostics, static x => x.Id == "SBM1004");
    }

    // ------------------------------------------------------------
    // SBM1005 : the binding cannot create the entity
    // ------------------------------------------------------------

    [Theory]
    [InlineData("public Created(string code) => Code = code;")]
    [InlineData("private Created() { }")]
    [InlineData("public required int Id { get; set; }")]
    public void Sbm1005EntityNotCreatableEmitsDiagnostic(string member)
    {
        var source = $$"""
            using System;
            using Smart.IO.ByteMapper;
            using Smart.IO.ByteMapper.AspNetCore;

            namespace Test;

            [Map(13)]
            public sealed class Created
            {
                {{member}}

                [MapText(0, 13)]
                public string Code { get; set; } = default!;
            }

            [ByteMapperEndpoint]
            public static partial class CreatedMappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, Created target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, Created source);
            }
            """;

        Assert.Equal(["SBM1005"], AspNetCoreGeneratorTestHelper.GetProblemIds(source));
    }

    // ------------------------------------------------------------
    // SBM1006 : endpoint class names differing only in case
    // ------------------------------------------------------------

    [Fact]
    public void Sbm1006CaseOnlyClassNamesGenerateTheFirstOnly()
    {
        var problems = AspNetCoreGeneratorTestHelper.GetProblemIds(Entity +
            """

            [ByteMapperEndpoint]
            public static partial class Mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }

            [ByteMapperEndpoint]
            public static partial class mappers
            {
                [ByteReader]
                public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

                [ByteWriter]
                public static partial void Write(Span<byte> destination, SampleData source);
            }
            """);

        Assert.Contains("SBM1006", problems);
        Assert.DoesNotContain("CS8785", problems);
    }
}
