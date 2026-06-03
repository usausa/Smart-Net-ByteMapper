namespace Smart.IO.ByteMapper.AspNetCore.Generator.Tests;

using Smart.IO.ByteMapper.AspNetCore.Generator;
using Smart.IO.ByteMapper.AspNetCore.Generator.Models;

using SourceGenerateHelper;

// Direct unit tests for the emit stage. EndpointModel records are constructed in-memory and the
// emitted source is asserted as a string — no Roslyn compilation/driver needed.
public class ByteMapperAspNetCoreSourceBuilderTests
{
    private static EndpointModel Endpoint(
        string? profileFqn = null,
        bool generateArray = true,
        string ns = "Test",
        string className = "SampleMappers",
        string entityFqn = "global::Test.Sample",
        int size = 8,
        string nameSuffix = "")
        => new(
            ns,
            className,
            entityFqn,
            "Read",
            "Write",
            size,
            profileFqn,
            generateArray,
            "Test",
            nameSuffix);

    private static string BuildBinding(EndpointModel ep)
    {
        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBinding(builder, ep);
        return builder.ToString();
    }

    private static string BuildBootstrap(params EndpointModel[] endpoints)
    {
        var builder = new SourceBuilder();
        ByteMapperAspNetCoreSourceBuilder.BuildBootstrap(builder, endpoints);
        return builder.ToString();
    }

    // -------------------------------------------------------
    // BuildBinding
    // -------------------------------------------------------

    [Fact]
    public void WhenBuildBindingThenEmitsPartialClass()
    {
        var src = BuildBinding(Endpoint());

        Assert.Contains("partial class SampleMappers", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenBuildBindingThenEmitsSingleFactoryWithSizeAndDelegates()
    {
        var src = BuildBinding(Endpoint());

        Assert.Contains("public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperBinding<global::Test.Sample> CreateByteMapperBinding()", src, StringComparison.Ordinal);
        Assert.Contains("size: 8,", src, StringComparison.Ordinal);
        Assert.Contains("read:    static (s, t) => Read(s, t),", src, StringComparison.Ordinal);
        Assert.Contains("write:   static (s, d) => Write(d, s),", src, StringComparison.Ordinal);
        Assert.Contains("factory: static () => new global::Test.Sample());", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenGenerateArrayBindingTrueThenEmitsArrayFactory()
    {
        var src = BuildBinding(Endpoint(generateArray: true));

        Assert.Contains("CreateByteMapperArrayBinding()", src, StringComparison.Ordinal);
        Assert.Contains("elementSize:  8,", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenGenerateArrayBindingFalseThenOmitsArrayFactory()
    {
        var src = BuildBinding(Endpoint(generateArray: false));

        Assert.DoesNotContain("CreateByteMapperArrayBinding", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenProfileSuffixThenEmitsSuffixedFactory()
    {
        var src = BuildBinding(Endpoint(profileFqn: "global::Test.MyProfile", nameSuffix: "_MyProfile"));

        Assert.Contains("CreateByteMapperBinding_MyProfile()", src, StringComparison.Ordinal);
        Assert.Contains("CreateByteMapperArrayBinding_MyProfile()", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenEntitySuffixThenEmitsEntitySuffixedFactory()
    {
        var src = BuildBinding(Endpoint(nameSuffix: "_Sample"));

        Assert.Contains("CreateByteMapperBinding_Sample()", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenNamespaceSetThenEmitsNamespace()
    {
        var src = BuildBinding(Endpoint());

        Assert.Contains("namespace Test", src, StringComparison.Ordinal);
    }

    // -------------------------------------------------------
    // BuildBootstrap
    // -------------------------------------------------------

    [Fact]
    public void WhenBuildBootstrapThenEmitsRegistryBuildAndExtension()
    {
        var src = BuildBootstrap(Endpoint());

        Assert.Contains("internal static class __ByteMapperAspNetCoreBootstrap", src, StringComparison.Ordinal);
        Assert.Contains("public static global::Smart.IO.ByteMapper.AspNetCore.ByteMapperRegistry Build()", src, StringComparison.Ordinal);
        Assert.Contains("AddByteMapperFormatters(", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenDefaultBindingThenProfileLiteralIsNull()
    {
        var src = BuildBootstrap(Endpoint(profileFqn: null));

        Assert.Contains("{ (typeof(global::Test.Sample), null), global::Test.SampleMappers.CreateByteMapperBinding() },", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenProfileBindingThenProfileLiteralIsTypeof()
    {
        var src = BuildBootstrap(Endpoint(profileFqn: "global::Test.MyProfile", nameSuffix: "_MyProfile"));

        Assert.Contains("{ (typeof(global::Test.Sample), typeof(global::Test.MyProfile)), global::Test.SampleMappers.CreateByteMapperBinding_MyProfile() },", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenArrayBindingDisabledThenArrayDictionaryEntryOmitted()
    {
        var src = BuildBootstrap(Endpoint(generateArray: false));

        Assert.DoesNotContain("CreateByteMapperArrayBinding", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenMultipleEndpointsThenBothRegistered()
    {
        var a = Endpoint(entityFqn: "global::Test.A", nameSuffix: "_A");
        var b = Endpoint(entityFqn: "global::Test.B", nameSuffix: "_B");

        var src = BuildBootstrap(a, b);

        Assert.Contains("global::Test.SampleMappers.CreateByteMapperBinding_A() },", src, StringComparison.Ordinal);
        Assert.Contains("global::Test.SampleMappers.CreateByteMapperBinding_B() },", src, StringComparison.Ordinal);
    }

    // -------------------------------------------------------
    // MakeFilename
    // -------------------------------------------------------

    [Fact]
    public void WhenNoSuffixThenFilenameIsNamespacedClass()
    {
        var name = ByteMapperAspNetCoreSourceBuilder.MakeFilename("Test.Ns", "SampleMappers", string.Empty);

        Assert.Equal("Test_Ns_SampleMappers", name);
    }

    [Fact]
    public void WhenSuffixThenFilenameIncludesSuffix()
    {
        var name = ByteMapperAspNetCoreSourceBuilder.MakeFilename("Test.Ns", "SampleMappers", "_MyProfile");

        Assert.Equal("Test_Ns_SampleMappers_MyProfile", name);
    }

    [Fact]
    public void WhenEntityAndProfileSuffixThenFilenameIncludesBoth()
    {
        var name = ByteMapperAspNetCoreSourceBuilder.MakeFilename("Test.Ns", "SampleMappers", "_EntityA_MyProfile");

        Assert.Equal("Test_Ns_SampleMappers_EntityA_MyProfile", name);
    }
}
