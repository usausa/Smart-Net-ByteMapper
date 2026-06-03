namespace Smart.IO.ByteMapper.Generator.Tests;

using Microsoft.CodeAnalysis;

using Smart.IO.ByteMapper.Generator;
using Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

// Direct unit tests for the emit stage. Models are constructed in-memory and the emitted source is
// asserted as a string — no Roslyn compilation/driver needed.
public class ByteMapperSourceBuilderTests
{
    private const string EntityFqn = "global::Test.Entity";

    private static MemberMappingModel Member(
        string name,
        int offset,
        int size,
        SizeKind sizeKind = SizeKind.Const,
        int? constSize = null,
        string converterFqn = "global::Test.IntConverter",
        params string[] ctorArgs)
        => new(name, offset, size, new ConverterCallModel(
            converterFqn,
            "Converter0_0",
            new EquatableArray<string>(ctorArgs),
            sizeKind,
            constSize ?? (sizeKind == SizeKind.Const ? size : null)));

    private static MapperMethodModel Method(
        MapperShape shape,
        string methodName,
        int size,
        MemberMappingModel[] members,
        TypeMappingModel[]? typeMappings = null,
        bool isValueType = false,
        string bufferParam = "buffer",
        string targetParam = "target")
        => new(
            "Test",
            "Mappers",
            isValueType,
            Accessibility.Public,
            methodName,
            shape,
            EntityFqn,
            size,
            bufferParam,
            targetParam,
            new EquatableArray<MemberMappingModel>(members),
            new EquatableArray<TypeMappingModel>(typeMappings ?? []),
            new EquatableArray<DiagnosticInfo>([]));

    private static string Build(params MapperMethodModel[] methods)
    {
        var builder = new SourceBuilder();
        ByteMapperSourceBuilder.Build(builder, methods);
        return builder.ToString();
    }

    [Fact]
    public void WhenClassNotValueTypeThenEmitsPartialClass()
    {
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4)]));

        Assert.Contains("partial class Mappers", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenValueTypeThenEmitsPartialStruct()
    {
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4)], isValueType: true));

        Assert.Contains("partial struct Mappers", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenInPlaceReaderThenEmitsReadAssignment()
    {
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4)]));

        Assert.Contains("static partial void Read(global::System.ReadOnlySpan<byte> buffer, global::Test.Entity target)", src, StringComparison.Ordinal);
        Assert.Contains("target.Id = Converter0.Read(buffer.Slice(0, 4));", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenNewInstanceReaderThenEmitsNewAndReturn()
    {
        var src = Build(Method(MapperShape.NewInstance, "ReadNew", 4, [Member("Id", 0, 4)]));

        Assert.Contains("var target = new global::Test.Entity();", src, StringComparison.Ordinal);
        Assert.Contains("return target;", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenWriteSpanThenEmitsWriteCall()
    {
        var src = Build(Method(MapperShape.WriteSpan, "Write", 4, [Member("Id", 0, 4)]));

        Assert.Contains("Converter0.Write(buffer.Slice(0, 4), target.Id);", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenWriteAllocThenEmitsBufferAllocationAndReturn()
    {
        var src = Build(Method(MapperShape.WriteAlloc, "WriteAlloc", 4, [Member("Id", 0, 4)]));

        Assert.Contains("var buffer = new byte[4];", src, StringComparison.Ordinal);
        Assert.Contains("var span = (global::System.Span<byte>)buffer;", src, StringComparison.Ordinal);
        Assert.Contains("return buffer;", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenConverterFieldThenEmittedAsStaticReadonly()
    {
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4, converterFqn: "global::Test.IntConverter")]));

        Assert.Contains("private static readonly global::Test.IntConverter Converter0 = new();", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenTwoMethodsShareConverterConfigThenSingleFieldEmitted()
    {
        var read = Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4)]);
        var write = Method(MapperShape.WriteSpan, "Write", 4, [Member("Id", 0, 4)]);

        var src = Build(read, write);

        // Deduplicated to a single shared field; Converter1 must not appear.
        var firstIndex = src.IndexOf("Converter0 = new()", StringComparison.Ordinal);
        Assert.NotEqual(-1, firstIndex);
        Assert.DoesNotContain("Converter1", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenFillerTypeMappingThenEmitsFill()
    {
        var tm = new TypeMappingModel(0, 4, TypeMappingKind.Filler, new EquatableArray<byte>([]), 0x20);
        var src = Build(Method(MapperShape.WriteSpan, "Write", 8, [Member("Id", 4, 4)], [tm]));

        Assert.Contains("buffer.Slice(0, 4).Fill((byte)0x20);", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenConstantTypeMappingThenEmitsStaticFieldAndCopy()
    {
        var tm = new TypeMappingModel(4, 2, TypeMappingKind.Constant, new EquatableArray<byte>([0x0D, 0x0A]), 0);
        var src = Build(Method(MapperShape.WriteSpan, "Write", 6, [Member("Id", 0, 4)], [tm]));

        Assert.Contains("private static readonly byte[] ConstantBytes0 = [0x0D, 0x0A];", src, StringComparison.Ordinal);
        Assert.Contains("new global::System.ReadOnlySpan<byte>(ConstantBytes0).CopyTo(buffer.Slice(4, 2));", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenStaticMemberSizeThenUsesConverterTypeSize()
    {
        var member = Member("Value", 0, 0, SizeKind.StaticMember, converterFqn: "global::Test.BinaryConverter");
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [member]));

        Assert.Contains("Converter0.Read(buffer.Slice(0, global::Test.BinaryConverter.Size));", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenInstanceSizeWithoutConstThenUsesFieldSize()
    {
        var member = Member("Value", 0, 0, SizeKind.Instance, converterFqn: "global::Test.TextConverter");
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [member]));

        Assert.Contains("Converter0.Read(buffer.Slice(0, Converter0.Size));", src, StringComparison.Ordinal);
    }

    [Fact]
    public void WhenNamespaceSetThenEmitsNamespace()
    {
        var src = Build(Method(MapperShape.InPlace, "Read", 4, [Member("Id", 0, 4)]));

        Assert.Contains("namespace Test", src, StringComparison.Ordinal);
    }
}
