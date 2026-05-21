namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SourceGenerateHelper;

using Smart.IO.ByteMapper.Generator.Helpers;
using Smart.IO.ByteMapper.Generator.Models;

[Generator]
public sealed class ByteMapperGenerator : IIncrementalGenerator
{
    private const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    private const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";
    private const string MapFillerAttributeName = "Smart.IO.ByteMapper.MapFillerAttribute";
    private const string MapConstantAttributeName = "Smart.IO.ByteMapper.MapConstantAttribute";
    private const string ByteMapperPropertyAttributeName = "Smart.IO.ByteMapper.ByteMapperPropertyAttribute";
    private const string ByteMapperConverterAttributeOpenName = "Smart.IO.ByteMapper.ByteMapperConverterAttribute`1";

    // default CRLF
    private static readonly byte[] DefaultDelimiter = [0x0D, 0x0A];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var readers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteReaderAttributeName,
                static (s, _) => s is MethodDeclarationSyntax,
                static (ctx, _) => ParseMethod(ctx, MapperKind.Reader))
            .Collect();

        var writers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ByteWriterAttributeName,
                static (s, _) => s is MethodDeclarationSyntax,
                static (ctx, _) => ParseMethod(ctx, MapperKind.Writer))
            .Collect();

        var methods = readers.Combine(writers)
            .Select(static (t, _) => t.Left.AddRange(t.Right));

        context.RegisterImplementationSourceOutput(
            methods,
            static (spc, items) => Execute(spc, items));
    }

    // -------------------------------------------------------
    // Parser
    // -------------------------------------------------------

    private static Result<MapperMethodModel> ParseMethod(GeneratorAttributeSyntaxContext context, MapperKind kind)
    {
        var syntax = (MethodDeclarationSyntax)context.TargetNode;
        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not IMethodSymbol symbol)
            return Results.Error<MapperMethodModel>(null);

        if (!symbol.IsStatic || !symbol.IsPartialDefinition)
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodDefinition, syntax.GetLocation(), symbol.Name));

        // Determine shape and target type
        var (shape, targetType, errors) = DetermineShape(symbol, kind);
        if (errors.Count > 0)
            return Results.Error<MapperMethodModel>(errors[0]);
        if (targetType == null)
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodSignature, syntax.GetLocation(), symbol.Name));

        // Check for SBM0014: return-value reader requires parameterless constructor
        if (shape == MapperShape.NewInstance)
        {
            var namedTarget = targetType as INamedTypeSymbol;
            var hasDefaultCtor = namedTarget != null && (namedTarget.IsValueType ||
                namedTarget.InstanceConstructors.Any(c => c.Parameters.Length == 0 && c.DeclaredAccessibility == Accessibility.Public));
            if (!hasDefaultCtor)
                return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.TargetNotInstantiatable, syntax.GetLocation(), symbol.Name));
        }

        // Get ByteReader/Writer attribute
        var methodAttr = symbol.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == (kind == MapperKind.Reader ? ByteReaderAttributeName : ByteWriterAttributeName));

        ITypeSymbol? profileType = null;
        var validateLayout = true;
        if (methodAttr != null)
        {
            foreach (var na in methodAttr.NamedArguments)
            {
                if (na.Key == "Profile" && na.Value.Value is ITypeSymbol pt) profileType = pt;
                if (na.Key == "ValidateLayout" && na.Value.Value is bool vl) validateLayout = vl;
            }
        }

        // Determine attribute source type (Profile or Target)
        var attrSourceType = profileType ?? targetType;

        // Get [Map] attribute from attribute source
        var mapAttr = attrSourceType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
        if (mapAttr == null)
        {
            var diagId = profileType != null ? Diagnostics.ProfileMissingMapAttribute : Diagnostics.MissingMapAttribute;
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(diagId, syntax.GetLocation(), symbol.Name));
        }

        var mapSize = (int)(mapAttr.ConstructorArguments[0].Value ?? 0);
        var autoFiller = true;
        var useDelimiter = true;
        byte? nullFiller = null;
        byte[]? delimiter = null;
        foreach (var na in mapAttr.NamedArguments)
        {
            if (na.Key == "AutoFiller" && na.Value.Value is bool af) autoFiller = af;
            if (na.Key == "UseDelimiter" && na.Value.Value is bool ud) useDelimiter = ud;
            if (na.Key == "NullFiller" && na.Value.Value is byte nf) nullFiller = nf;
            if (na.Key == "Delimiter" && na.Value.Kind == TypedConstantKind.Array)
                delimiter = na.Value.Values.Select(v => (byte)(v.Value ?? 0)).ToArray();
        }

        var containingType = symbol.ContainingType;
        var ns = string.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        // Collect fillers and constants from type attributes
        var typeMappings = CollectTypeMappings(attrSourceType);

        // Collect member mappings
        var compilation = context.SemanticModel.Compilation;
        var propertyAttrBase = compilation.GetTypeByMetadataName(ByteMapperPropertyAttributeName);
        var converterAttrBase = compilation.GetTypeByMetadataName(ByteMapperConverterAttributeOpenName);

        if (propertyAttrBase == null || converterAttrBase == null)
            return Results.Error<MapperMethodModel>(null);

        var diagErrors = new List<DiagnosticInfo>();
        var members = CollectMembers(
            symbol, attrSourceType, targetType, profileType, propertyAttrBase, converterAttrBase,
            syntax, diagErrors);

        // Layout resolution
        var delimiterBytes = useDelimiter ? (delimiter ?? DefaultDelimiter) : Array.Empty<byte>();
        var fillerByte = (byte)0x20;
        ResolveLayout(members, typeMappings, mapSize, delimiterBytes, autoFiller, fillerByte, validateLayout, attrSourceType.Name, syntax, diagErrors);

        // Determine method index placeholder (will be assigned by Execute)
        var layoutModel = new LayoutModel(
            mapSize,
            fillerByte,
            nullFiller ?? 0x20,
            useDelimiter,
            new EquatableArray<byte>(delimiterBytes),
            autoFiller,
            validateLayout);

        var className = SourceGenerateHelper.SymbolExtensions.GetClassName(containingType);
        var model = new MapperMethodModel(
            ns,
            className,
            containingType.IsValueType,
            symbol.DeclaredAccessibility,
            symbol.Name,
            kind,
            shape,
            targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            profileType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            0, // MethodIndex assigned in Execute
            layoutModel,
            new EquatableArray<MemberMappingModel>(members.ToArray()),
            new EquatableArray<TypeMappingModel>(typeMappings.ToArray()),
            new EquatableArray<DiagnosticInfo>(diagErrors.ToArray()));

        return Results.Success(model);
    }

    private static (MapperShape shape, ITypeSymbol? targetType, List<DiagnosticInfo> errors) DetermineShape(
        IMethodSymbol symbol, MapperKind kind)
    {
        var errors = new List<DiagnosticInfo>();
        var syntax = symbol.DeclaringSyntaxReferences[0].GetSyntax();

        if (kind == MapperKind.Reader)
        {
            if (symbol.ReturnsVoid && symbol.Parameters.Length == 2)
            {
                // void Read(ReadOnlySpan<byte> buffer, T target)
                return (MapperShape.InPlace, symbol.Parameters[1].Type, errors);
            }
            if (!symbol.ReturnsVoid && symbol.Parameters.Length == 1)
            {
                // T Read(ReadOnlySpan<byte> buffer)
                return (MapperShape.NewInstance, symbol.ReturnType, errors);
            }
        }
        else
        {
            if (symbol.ReturnsVoid && symbol.Parameters.Length == 2)
            {
                // void Write(T source, Span<byte> buffer)
                return (MapperShape.WriteSpan, symbol.Parameters[0].Type, errors);
            }
            if (!symbol.ReturnsVoid && symbol.Parameters.Length == 1)
            {
                // byte[] Write(T source)
                return (MapperShape.WriteAlloc, symbol.Parameters[0].Type, errors);
            }
        }

        errors.Add(new DiagnosticInfo(Diagnostics.InvalidMethodSignature, syntax.GetLocation(), symbol.Name));
        return (MapperShape.InPlace, null, errors);
    }

    private static List<TypeMappingModel> CollectTypeMappings(ITypeSymbol type)
    {
        var result = new List<TypeMappingModel>();
        foreach (var attr in type.GetAttributes())
        {
            var attrClass = attr.AttributeClass?.ToDisplayString();
            if (attrClass == MapFillerAttributeName)
            {
                var offset = (int)(attr.ConstructorArguments[0].Value ?? 0);
                var length = (int)(attr.ConstructorArguments[1].Value ?? 0);
                var filler = (byte)0x20;
                foreach (var na in attr.NamedArguments)
                    if (na.Key == "Filler" && na.Value.Value is byte f) filler = f;
                result.Add(new TypeMappingModel(offset, length, TypeMappingKind.Filler, new EquatableArray<byte>(Array.Empty<byte>()), filler));
            }
            else if (attrClass == MapConstantAttributeName)
            {
                var offset = (int)(attr.ConstructorArguments[0].Value ?? 0);
                var content = attr.ConstructorArguments[1].Values.Select(v => (byte)(v.Value ?? 0)).ToArray();
                result.Add(new TypeMappingModel(offset, content.Length, TypeMappingKind.Constant, new EquatableArray<byte>(content), 0));
            }
        }
        return result;
    }

    private static List<MemberMappingModel> CollectMembers(
        IMethodSymbol methodSymbol,
        ITypeSymbol attrSourceType,
        ITypeSymbol targetType,
        ITypeSymbol? profileType,
        INamedTypeSymbol propertyAttrBase,
        INamedTypeSymbol converterAttrBase,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        var members = new List<MemberMappingModel>();
        var propertyIndex = 0;

        // Walk properties in attribute source type
        foreach (var member in attrSourceType.GetMembers().OfType<IPropertySymbol>())
        {
            foreach (var attr in member.GetAttributes())
            {
                if (attr.AttributeClass == null) continue;
                if (!attr.AttributeClass.InheritsFrom(propertyAttrBase)) continue;

                var offset = (int)(attr.ConstructorArguments[0].Value ?? 0);

                // Determine actual property symbol on target
                var targetProp = member;
                if (profileType != null)
                {
                    var found = targetType.GetMembers(member.Name).OfType<IPropertySymbol>().FirstOrDefault();
                    if (found == null)
                    {
                        errors.Add(new DiagnosticInfo(Diagnostics.ProfilePropertyNotFound, syntax.GetLocation(), $"{methodSymbol.Name}, {member.Name}"));
                        continue;
                    }
                    targetProp = found;
                }

                // Try to get Converter type from ByteMapperConverterAttribute<TConverter>
                var converterBase = attr.AttributeClass.FindConverterAttributeBase(converterAttrBase);
                if (converterBase == null) continue; // MapArray or unrecognized - skip for now

                var converterType = converterBase.TypeArguments[0];
                var converterFqn = converterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                // Build ctor arg expressions
                var ctorArgs = BuildConverterCtorArgs(attr, converterType, errors, syntax, methodSymbol.Name, member.Name);

                // Determine size
                var (sizeKind, constSize) = DetermineConverterSize(converterType, ctorArgs);

                var fieldName = $"Converter0_{propertyIndex}"; // MethodIndex will be fixed in Execute
                var converterCall = new ConverterCallModel(
                    converterFqn,
                    fieldName,
                    new EquatableArray<string>(ctorArgs.ToArray()),
                    sizeKind,
                    constSize);

                var propTypeFqn = targetProp.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var isNullable = targetProp.Type.IsNullableSymbol();
                var size = constSize ?? 0; // will be fixed for instance size converters

                members.Add(new MemberMappingModel(
                    targetProp.Name,
                    propTypeFqn,
                    isNullable,
                    offset,
                    size,
                    propertyIndex,
                    converterCall));

                propertyIndex++;
                break; // Only first attribute per property
            }
        }

        return members;
    }

    private static List<string> BuildConverterCtorArgs(
        AttributeData attr,
        ITypeSymbol converterType,
        List<DiagnosticInfo> errors,
        MethodDeclarationSyntax syntax,
        string methodName,
        string propertyName)
    {
        // Collect ctor args from attribute: skip first arg (offset)
        var args = new List<string>();
        for (var i = 1; i < attr.ConstructorArguments.Length; i++)
            args.Add(attr.ConstructorArguments[i].ToLiteralExpression());

        // Build lookup of named arguments (user-specified)
        var namedArgs = attr.NamedArguments.ToDictionary(na => na.Key, na => na.Value.ToLiteralExpression());

        // Build lookup of attribute property defaults from attribute class property initializers
        var attrPropDefaults = GetAttributePropertyDefaults(attr.AttributeClass);

        if (converterType is INamedTypeSymbol namedConverter)
        {
            var ctor = namedConverter.InstanceConstructors.FirstOrDefault(c => c.DeclaredAccessibility == Accessibility.Public);
            if (ctor != null)
            {
                // Map converter ctor params (skip already-covered positional params)
                // attr.ConstructorArguments includes offset at [0]; we skip that and add [1..] to args above.
                // So Converter ctor params [0 .. (attr.ConstructorArguments.Length-2)] are already covered.
                var coveredCount = attr.ConstructorArguments.Length - 1; // number of converter ctor params already filled
                for (var i = coveredCount; i < ctor.Parameters.Length; i++)
                {
                    var param = ctor.Parameters[i];
                    var pascalName = param.Name.Length > 0
                        ? param.Name.Substring(0, 1).ToUpperInvariant() + param.Name.Substring(1)
                        : param.Name;

                    // 1) User-specified named argument (pascal or camel)
                    if (namedArgs.TryGetValue(pascalName, out var val) || namedArgs.TryGetValue(param.Name, out val))
                    {
                        args.Add(val);
                        continue;
                    }

                    // 2) Attribute property default value
                    if (attrPropDefaults.TryGetValue(pascalName, out val))
                    {
                        args.Add(val);
                        continue;
                    }

                    // 3) Converter ctor parameter explicit default
                    args.Add(GetDefaultLiteral(param));
                }
            }
        }

        return args;
    }

    /// <summary>
    /// Reads the default values from attribute class property initializers (e.g. Endian Endian { get; init; } = Endian.Big).
    /// Returns pascal-cased property name → C# literal expression.
    /// </summary>
    private static Dictionary<string, string> GetAttributePropertyDefaults(INamedTypeSymbol? attrClass)
    {
        var result = new Dictionary<string, string>();
        if (attrClass == null) return result;

        foreach (var member in attrClass.GetMembers())
        {
            if (member is not IPropertySymbol prop) continue;
            if (prop.IsStatic || prop.DeclaredAccessibility != Accessibility.Public) continue;

            foreach (var syntaxRef in prop.DeclaringSyntaxReferences)
            {
                var node = syntaxRef.GetSyntax();
                if (node is Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax propSyntax
                    && propSyntax.Initializer?.Value != null)
                {
                    result[prop.Name] = propSyntax.Initializer.Value.ToString();
                    break;
                }
            }
        }

        return result;
    }


    private static string GetDefaultLiteral(IParameterSymbol param)
    {
        if (param.HasExplicitDefaultValue)
        {
            if (param.Type.TypeKind == TypeKind.Enum && param.ExplicitDefaultValue != null)
            {
                var fqn = param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                // Find enum member name
                foreach (var member in param.Type.GetMembers())
                {
                    if (member is IFieldSymbol f && f.HasConstantValue && Equals(f.ConstantValue, param.ExplicitDefaultValue))
                        return $"{fqn}.{f.Name}";
                }
                return $"({fqn}){param.ExplicitDefaultValue}";
            }
            return param.ExplicitDefaultValue switch
            {
                null => "null",
                bool b => b ? "true" : "false",
                byte bt => $"(byte)0x{bt:X2}",
                string s => $"\"{s}\"",
                _ => param.ExplicitDefaultValue.ToString() ?? "default"
            };
        }
        return "default";
    }

    private static (SizeKind sizeKind, int? constSize) DetermineConverterSize(ITypeSymbol converterType, List<string> ctorArgs)
    {
        if (converterType is not INamedTypeSymbol namedConverter) return (SizeKind.Instance, null);

        // Check for const Size field or static readonly Size field
        foreach (var member in namedConverter.GetMembers("Size"))
        {
            if (member is IFieldSymbol field)
            {
                if (field.IsConst && field.HasConstantValue)
                    return (SizeKind.Const, Convert.ToInt32(field.ConstantValue));
                // static readonly int Size = Unsafe.SizeOf<T>() - value not known at compile time
                    // treat as StaticMember so emitter uses ConverterTypeFqn.Size
                    if (field.IsStatic && field.IsReadOnly)
                    {
                        return (SizeKind.StaticMember, null);
                    }
            }
            if (member is IPropertySymbol { IsStatic: false })
            {
                // Instance Size property - try to determine from ctor args (first arg is length)
                if (ctorArgs.Count > 0 && int.TryParse(ctorArgs[0], out var len))
                    return (SizeKind.Instance, len);
                return (SizeKind.Instance, null);
            }
        }

        // Fallback: use first ctor arg as size
        if (ctorArgs.Count > 0 && int.TryParse(ctorArgs[0], out var fallback))
            return (SizeKind.Instance, fallback);

        return (SizeKind.Instance, null);
    }

    private static void ResolveLayout(
        List<MemberMappingModel> members,
        List<TypeMappingModel> typeMappings,
        int mapSize,
        byte[] delimiter,
        bool autoFiller,
        byte fillerByte,
        bool validateLayout,
        string typeName,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        // Add delimiter as constant at end
        if (delimiter.Length > 0)
        {
            var delimOffset = mapSize - delimiter.Length;
            typeMappings.Add(new TypeMappingModel(delimOffset, delimiter.Length, TypeMappingKind.Constant,
                new EquatableArray<byte>(delimiter), 0));
        }

        // Sort members by offset
        members.Sort((a, b) => a.Offset.CompareTo(b.Offset));
        typeMappings.Sort((a, b) => a.Offset.CompareTo(b.Offset));

        // Validate overlap
        if (validateLayout)
        {
            var allRanges = members.Select(m => (m.Offset, m.Size))
                .Concat(typeMappings.Select(t => (t.Offset, t.Size)))
                .OrderBy(r => r.Offset)
                .ToList();

            for (var i = 0; i < allRanges.Count - 1; i++)
            {
                var end = allRanges[i].Offset + allRanges[i].Size;
                if (end > allRanges[i + 1].Offset)
                    errors.Add(new DiagnosticInfo(Diagnostics.RangeOverlap, syntax.GetLocation(), typeName));
            }
        }
    }

    // -------------------------------------------------------
    // Generator
    // -------------------------------------------------------

    private static void Execute(SourceProductionContext context, ImmutableArray<Result<MapperMethodModel>> results)
    {
        foreach (var error in results.SelectError())
            context.ReportDiagnostic(error);

        var builder = new SourceBuilder();

        var methodsByClass = results.SelectValue()
            .GroupBy(m => new { m.Namespace, m.ClassName });

        foreach (var group in methodsByClass)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            // Report per-method diagnostics
            foreach (var m in group)
            {
                foreach (var err in m.Errors.ToArray())
                    context.ReportDiagnostic(err);
            }

            // Assign method indices within the group
            var methods = group.ToList();
            var numberedMethods = new List<MapperMethodModel>();
            for (var i = 0; i < methods.Count; i++)
            {
                var m = methods[i];
                // Reassign field names with correct method index
                var fixedMembers = m.Members.ToArray().Select((member, pi) =>
                    member with
                    {
                        PropertyIndex = pi,
                        Converter = member.Converter with { FieldName = $"Converter{i}_{pi}" }
                    }).ToArray();
                numberedMethods.Add(m with
                {
                    MethodIndex = i,
                    Members = new EquatableArray<MemberMappingModel>(fixedMembers)
                });
            }

            builder.Clear();
            BuildSource(builder, numberedMethods);

            var filename = MakeFilename(group.Key.Namespace, group.Key.ClassName);
            context.AddSource(filename, SourceText.From(builder.ToString(), Encoding.UTF8));
        }
    }

    private static void BuildSource(SourceBuilder builder, List<MapperMethodModel> methods)
    {
        var first = methods[0];

        builder.AutoGenerated();
        builder.EnableNullable();
        builder.NewLine();

        if (!string.IsNullOrEmpty(first.Namespace))
        {
            builder.Namespace(first.Namespace);
            builder.NewLine();
        }

        builder.Indent()
            .Append("partial ")
            .Append(first.IsValueType ? "struct " : "class ")
            .Append(first.ClassName)
            .NewLine();
        builder.BeginScope();

        // Emit all converter fields first
        foreach (var method in methods)
        {
            foreach (var member in method.Members.ToArray())
            {
                builder.Indent()
                    .Append("// [")
                    .Append(member.PropertyName)
                    .Append("] offset=")
                    .Append(member.Offset.ToString())
                    .NewLine();
                builder.Indent()
                    .Append("private static readonly ")
                    .Append(member.Converter.ConverterTypeFqn)
                    .Append(" ")
                    .Append(member.Converter.FieldName)
                    .Append(" = new(")
                    .Append(string.Join(", ", member.Converter.CtorArgExpressions.ToArray()))
                    .Append(");")
                    .NewLine();
            }
        }

        // Emit methods
        var methodFirst = true;
        foreach (var method in methods)
        {
            if (!methodFirst) builder.NewLine();
            methodFirst = false;

            builder.NewLine();
            EmitMethod(builder, method);
        }

        builder.EndScope();
    }

    private static void EmitMethod(SourceBuilder builder, MapperMethodModel method)
    {
        builder.Indent()
            .Append("[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]")
            .NewLine();

        var accessibility = method.MethodAccessibility.ToText();

        switch (method.Shape)
        {
            case MapperShape.InPlace:
                builder.Indent()
                    .Append(accessibility).Append(" static partial void ")
                    .Append(method.MethodName)
                    .Append("(global::System.ReadOnlySpan<byte> buffer, ")
                    .Append(method.TargetTypeFqn).Append(" target)")
                    .NewLine();
                builder.BeginScope();
                if (method.Layout.Validation)
                {
                    builder.Indent()
                        .Append($"if (buffer.Length < {method.Layout.Size}) throw new global::Smart.IO.ByteMapper.ByteMapperException(\"Buffer too small.\");")
                        .NewLine();
                }
                foreach (var member in method.Members.ToArray())
                {
                    EmitReadMember(builder, member);
                }
                builder.EndScope();
                break;

            case MapperShape.NewInstance:
                builder.Indent()
                    .Append(accessibility).Append(" static partial ")
                    .Append(method.TargetTypeFqn).Append(" ")
                    .Append(method.MethodName)
                    .Append("(global::System.ReadOnlySpan<byte> buffer)")
                    .NewLine();
                builder.BeginScope();
                if (method.Layout.Validation)
                {
                    builder.Indent()
                        .Append($"if (buffer.Length < {method.Layout.Size}) throw new global::Smart.IO.ByteMapper.ByteMapperException(\"Buffer too small.\");")
                        .NewLine();
                }
                builder.Indent()
                    .Append("var target = new ").Append(method.TargetTypeFqn).Append("();")
                    .NewLine();
                foreach (var member in method.Members.ToArray())
                {
                    EmitReadMember(builder, member);
                }
                builder.Indent().Append("return target;").NewLine();
                builder.EndScope();
                break;

            case MapperShape.WriteSpan:
                builder.Indent()
                    .Append(accessibility).Append(" static partial void ")
                    .Append(method.MethodName)
                    .Append("(").Append(method.TargetTypeFqn).Append(" source, global::System.Span<byte> buffer)")
                    .NewLine();
                builder.BeginScope();
                if (method.Layout.Validation)
                {
                    builder.Indent()
                        .Append($"if (buffer.Length < {method.Layout.Size}) throw new global::Smart.IO.ByteMapper.ByteMapperException(\"Buffer too small.\");")
                        .NewLine();
                }
                EmitWriteBody(builder, method);
                builder.EndScope();
                break;

            case MapperShape.WriteAlloc:
                builder.Indent()
                    .Append(accessibility).Append(" static partial byte[] ")
                    .Append(method.MethodName)
                    .Append("(").Append(method.TargetTypeFqn).Append(" source)")
                    .NewLine();
                builder.BeginScope();
                builder.Indent()
                    .Append($"var buffer = new byte[{method.Layout.Size}];")
                    .NewLine();
                builder.Indent()
                    .Append("var span = (global::System.Span<byte>)buffer;")
                    .NewLine();
                EmitWriteBody(builder, method, spanVarName: "span");
                builder.Indent().Append("return buffer;").NewLine();
                builder.EndScope();
                break;
        }
    }

    private static void EmitReadMember(SourceBuilder builder, MemberMappingModel member)
    {
        string size;
        if (member.Converter.SizeKind == SizeKind.Const)
            size = member.Converter.ConstSize!.Value.ToString();
        else if (member.Converter.SizeKind == SizeKind.StaticMember)
            size = $"{member.Converter.ConverterTypeFqn}.Size";
        else if (member.Converter.ConstSize.HasValue)
            size = member.Converter.ConstSize.Value.ToString();
        else
            size = $"{member.Converter.FieldName}.Size";

        builder.Indent()
            .Append("target.")
            .Append(member.PropertyName)
            .Append(" = ")
            .Append(member.Converter.FieldName)
            .Append(".Read(buffer.Slice(")
            .Append(member.Offset.ToString())
            .Append(", ")
            .Append(size)
            .Append("));")
            .NewLine();
    }

    private static void EmitWriteBody(SourceBuilder builder, MapperMethodModel method, string spanVarName = "buffer")
    {
        // Write constants/fillers
        foreach (var tm in method.TypeMappings.ToArray())
        {
            if (tm.Kind == TypeMappingKind.Filler)
            {
                builder.Indent()
                    .Append($"{spanVarName}.Slice({tm.Offset}, {tm.Size}).Fill((byte)0x{tm.Filler:X2});")
                    .NewLine();
            }
            else if (tm.Kind == TypeMappingKind.Constant)
            {
                var bytes = string.Join(", ", tm.Constant.ToArray().Select(b => $"0x{b:X2}"));
                builder.Indent()
                    .Append($"{{ var c = new byte[] {{ {bytes} }}; c.CopyTo({spanVarName}.Slice({tm.Offset}, {tm.Size})); }}")
                    .NewLine();
            }
        }

        foreach (var member in method.Members.ToArray())
        {
            string size;
            if (member.Converter.SizeKind == SizeKind.Const)
                size = member.Converter.ConstSize!.Value.ToString();
            else if (member.Converter.SizeKind == SizeKind.StaticMember)
                size = $"{member.Converter.ConverterTypeFqn}.Size";
            else if (member.Converter.ConstSize.HasValue)
                size = member.Converter.ConstSize.Value.ToString();
            else
                size = $"{member.Converter.FieldName}.Size";

            builder.Indent()
                .Append(member.Converter.FieldName)
                .Append($".Write({spanVarName}.Slice(")
                .Append(member.Offset.ToString())
                .Append(", ")
                .Append(size)
                .Append("), source.")
                .Append(member.PropertyName)
                .Append(");")
                .NewLine();
        }
    }

    // -------------------------------------------------------
    // Helper
    // -------------------------------------------------------

    private static string MakeFilename(string ns, string className)
    {
        var buffer = new StringBuilder();
        if (!string.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }
        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        buffer.Append(".g.cs");
        return buffer.ToString();
    }
}
