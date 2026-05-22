namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Smart.IO.ByteMapper.Generator.Helpers;
using Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

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
    private const string ConverterSupportedTypesAttributeName = "Smart.IO.ByteMapper.ConverterSupportedTypesAttribute";
    // Sizes of well-known unmanaged primitive types (used to resolve BinaryConverter<T>.Size at code-gen time)
    private static readonly Dictionary<string, int> KnownUnmanagedSizes = new(StringComparer.Ordinal)
    {
        ["byte"] = 1,
        ["sbyte"] = 1,
        ["short"] = 2,
        ["ushort"] = 2,
        ["int"] = 4,
        ["uint"] = 4,
        ["long"] = 8,
        ["ulong"] = 8,
        ["float"] = 4,
        ["double"] = 8,
        ["decimal"] = 16,
        ["System.Byte"] = 1,
        ["System.SByte"] = 1,
        ["System.Int16"] = 2,
        ["System.UInt16"] = 2,
        ["System.Int32"] = 4,
        ["System.UInt32"] = 4,
        ["System.Int64"] = 8,
        ["System.UInt64"] = 8,
        ["System.Single"] = 4,
        ["System.Double"] = 8,
        ["System.Decimal"] = 16
    };

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
        {
            return Results.Error<MapperMethodModel>(null);
        }

        if (!symbol.IsStatic || !symbol.IsPartialDefinition)
        {
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodDefinition, syntax.GetLocation(), symbol.Name));
        }

        // Determine shape and target type
        var (shape, targetType, bufferParamName, targetParamName, errors) = DetermineShape(symbol, kind);
        if (errors.Count > 0)
        {
            return Results.Error<MapperMethodModel>(errors[0]);
        }

        if (targetType == null)
        {
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodSignature, syntax.GetLocation(), symbol.Name));
        }

        // Check for SBM0014: return-value reader requires parameterless constructor
        if (shape == MapperShape.NewInstance)
        {
            if (targetType is INamedTypeSymbol namedTarget)
            {
                var hasDefaultCtor = namedTarget.IsValueType ||
                    namedTarget.InstanceConstructors.Any(c => (c.Parameters.Length == 0) && (c.DeclaredAccessibility == Accessibility.Public));
                if (!hasDefaultCtor)
                {
                    return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.TargetNotInstantiatable, syntax.GetLocation(), symbol.Name));
                }
            }
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
                if (na.Key == "Profile" && na.Value.Value is ITypeSymbol pt)
                {
                    profileType = pt;
                }

                if (na.Key == "ValidateLayout" && na.Value.Value is bool vl)
                {
                    validateLayout = vl;
                }
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

        var containingType = symbol.ContainingType;
        var ns = String.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        // Collect fillers and constants from type attributes
        var typeMappings = CollectTypeMappings(attrSourceType);

        // Collect member mappings
        var compilation = context.SemanticModel.Compilation;
        var propertyAttrBase = compilation.GetTypeByMetadataName(ByteMapperPropertyAttributeName);
        var converterAttrBase = compilation.GetTypeByMetadataName(ByteMapperConverterAttributeOpenName);

        if (propertyAttrBase == null || converterAttrBase == null)
        {
            return Results.Error<MapperMethodModel>(null);
        }

        var diagErrors = new List<DiagnosticInfo>();
        var members = CollectMembers(
            symbol,
            attrSourceType,
            targetType,
            profileType,
            propertyAttrBase,
            converterAttrBase,
            syntax,
            diagErrors);

        // Layout resolution
        ResolveLayout(
            members,
            typeMappings,
            validateLayout,
            attrSourceType.Name,
            syntax,
            diagErrors);

        var className = SourceGenerateHelper.SymbolExtensions.GetClassName(containingType);
        var model = new MapperMethodModel(
            ns,
            className,
            containingType.IsValueType,
            symbol.DeclaredAccessibility,
            symbol.Name,
            shape,
            targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            mapSize,
            bufferParamName,
            targetParamName,
            new EquatableArray<MemberMappingModel>(members.ToArray()),
            new EquatableArray<TypeMappingModel>(typeMappings.ToArray()),
            new EquatableArray<DiagnosticInfo>(diagErrors.ToArray()));

        return Results.Success(model);
    }

    private static bool IsReadOnlySpanOfByte(ITypeSymbol type)
    {
        return type is INamedTypeSymbol { Name: "ReadOnlySpan", IsGenericType: true } named
            && named.ContainingNamespace.ToDisplayString() == "System"
            && named.TypeArguments.Length == 1
            && named.TypeArguments[0].SpecialType == SpecialType.System_Byte;
    }

    private static bool IsSpanOfByte(ITypeSymbol type)
    {
        return type is INamedTypeSymbol { Name: "Span", IsGenericType: true } named
            && named.ContainingNamespace.ToDisplayString() == "System"
            && named.TypeArguments.Length == 1
            && named.TypeArguments[0].SpecialType == SpecialType.System_Byte;
    }

    private static (MapperShape Shape, ITypeSymbol? TargetType, string BufferParamName, string TargetParamName, List<DiagnosticInfo> Errors) DetermineShape(
        IMethodSymbol symbol, MapperKind kind)
    {
        var errors = new List<DiagnosticInfo>();
        var syntax = symbol.DeclaringSyntaxReferences[0].GetSyntax();

        if (kind == MapperKind.Reader)
        {
            // void Read(ReadOnlySpan<byte> source, T target)
            if (symbol.ReturnsVoid
                && symbol.Parameters.Length == 2
                && IsReadOnlySpanOfByte(symbol.Parameters[0].Type))
            {
                return (MapperShape.InPlace, symbol.Parameters[1].Type, symbol.Parameters[0].Name, symbol.Parameters[1].Name, errors);
            }

            // T Read(ReadOnlySpan<byte> source)
            if (!symbol.ReturnsVoid
                && symbol.Parameters.Length == 1
                && IsReadOnlySpanOfByte(symbol.Parameters[0].Type))
            {
                return (MapperShape.NewInstance, symbol.ReturnType, symbol.Parameters[0].Name, "target", errors);
            }
        }
        else
        {
            // void Write(Span<byte> destination, T source)
            if (symbol.ReturnsVoid
                && symbol.Parameters.Length == 2
                && IsSpanOfByte(symbol.Parameters[0].Type))
            {
                return (MapperShape.WriteSpan, symbol.Parameters[1].Type, symbol.Parameters[0].Name, symbol.Parameters[1].Name, errors);
            }

            // byte[] Write(T source)
            if (!symbol.ReturnsVoid
                && symbol.Parameters.Length == 1
                && symbol.ReturnType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte })
            {
                return (MapperShape.WriteAlloc, symbol.Parameters[0].Type, "buffer", symbol.Parameters[0].Name, errors);
            }
        }

        errors.Add(new DiagnosticInfo(Diagnostics.InvalidMethodSignature, syntax.GetLocation(), symbol.Name));
        return (MapperShape.InPlace, null, "buffer", "target", errors);
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
                {
                    if ((na.Key == "Filler") && (na.Value.Value is byte f))
                    {
                        filler = f;
                    }
                }
                result.Add(new TypeMappingModel(offset, length, TypeMappingKind.Filler, new EquatableArray<byte>([]), filler));
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
                if (attr.AttributeClass == null)
                {
                    continue;
                }

                if (!attr.AttributeClass.InheritsFrom(propertyAttrBase))
                {
                    continue;
                }

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
                if (converterBase == null)
                {
                    continue; // MapArray or unrecognized - skip for now
                }

                var converterType = converterBase.TypeArguments[0];
                var converterFqn = converterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                // SBM0008: check [ConverterSupportedTypes] on the attribute class
                if (!CheckSupportedTypes(attr.AttributeClass, targetProp.Type, syntax, methodSymbol, targetProp, errors))
                {
                    break;
                }

                // Build ctor arg expressions
                var ctorArgs = BuildConverterCtorArgs(attr, converterType);

                // Determine size
                var (sizeKind, constSize) = DetermineConverterSize(converterType, ctorArgs);

                var fieldName = $"Converter0_{propertyIndex}"; // MethodIndex will be fixed in Execute
                var converterCall = new ConverterCallModel(
                    converterFqn,
                    fieldName,
                    new EquatableArray<string>(ctorArgs.ToArray()),
                    sizeKind,
                    constSize);

                var size = constSize ?? 0; // will be fixed for instance size converters

                members.Add(new MemberMappingModel(
                    targetProp.Name,
                    offset,
                    size,
                    converterCall));

                propertyIndex++;
                break; // Only first attribute per property
            }
        }

        return members;
    }

    // Checks [ConverterSupportedTypes] on the attribute class against the target property type.
    // Returns false (and adds SBM0008) when the type is not supported.
    private static bool CheckSupportedTypes(
        INamedTypeSymbol attrClass,
        ITypeSymbol propType,
        MethodDeclarationSyntax syntax,
        IMethodSymbol methodSymbol,
        IPropertySymbol prop,
        List<DiagnosticInfo> errors)
    {
        var supportedAttr = attrClass.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ConverterSupportedTypesAttributeName);

        if (supportedAttr == null)
        {
            // No restriction declared — always allowed
            return true;
        }

        // Fixed type list: property type must be in Types[]
        var propTypeFqn = propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        foreach (var typeConst in supportedAttr.ConstructorArguments[0].Values)
        {
            if (typeConst.Value is ITypeSymbol allowedType)
            {
                if (allowedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == propTypeFqn)
                {
                    return true;
                }
            }
        }

        errors.Add(new DiagnosticInfo(Diagnostics.UnsupportedBinaryType, syntax.GetLocation(), $"{methodSymbol.Name}, {prop.Name}"));
        return false;
    }

    private static List<string> BuildConverterCtorArgs(
        AttributeData attr,
        ITypeSymbol converterType)
    {
        // Collect ctor args from attribute: skip first arg (offset)
        var args = new List<string>();
        for (var i = 1; i < attr.ConstructorArguments.Length; i++)
        {
            args.Add(attr.ConstructorArguments[i].ToLiteralExpression());
        }

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

    // Reads the default values from attribute class property initializers.
    // Returns pascal-cased property name → C# literal expression.
    private static Dictionary<string, string> GetAttributePropertyDefaults(INamedTypeSymbol? attrClass)
    {
        var result = new Dictionary<string, string>();
        if (attrClass == null)
        {
            return result;
        }

        foreach (var member in attrClass.GetMembers())
        {
            if (member is not IPropertySymbol prop)
            {
                continue;
            }

            if (prop.IsStatic || (prop.DeclaredAccessibility != Accessibility.Public))
            {
                continue;
            }

            foreach (var syntaxRef in prop.DeclaringSyntaxReferences)
            {
                var node = syntaxRef.GetSyntax();
                if (node is PropertyDeclarationSyntax propSyntax
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
                    {
                        return $"{fqn}.{f.Name}";
                    }
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

    private static (SizeKind SizeKind, int? ConstSize) DetermineConverterSize(ITypeSymbol converterType, List<string> ctorArgs)
    {
        if (converterType is not INamedTypeSymbol namedConverter)
        {
            return (SizeKind.Instance, null);
        }

        // Check for const Size field or static readonly Size field
        foreach (var member in namedConverter.GetMembers("Size"))
        {
            if (member is IFieldSymbol field)
            {
                if (field.IsConst && field.HasConstantValue)
                {
                    return (SizeKind.Const, Convert.ToInt32(field.ConstantValue, System.Globalization.CultureInfo.InvariantCulture));
                }

                // static readonly int Size = Unsafe.SizeOf<T>() - value not known at compile time
                // For generic converters (e.g. BinaryConverter<int>), resolve the type argument size if possible
                if (field.IsStatic && field.IsReadOnly)
                {
                    if (namedConverter.IsGenericType && namedConverter.TypeArguments.Length == 1)
                    {
                        var typeArg = namedConverter.TypeArguments[0];
                        var typeKey = typeArg.SpecialType != SpecialType.None
                            ? typeArg.ToDisplayString()
                            : typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", string.Empty);
                        if (KnownUnmanagedSizes.TryGetValue(typeKey, out var knownSize))
                        {
                            return (SizeKind.Const, knownSize);
                        }
                    }

                    return (SizeKind.StaticMember, null);
                }
            }

            if (member is IPropertySymbol { IsStatic: false })
            {
                // Instance Size property - try to determine from ctor args (first arg is length)
                if ((ctorArgs.Count > 0) && Int32.TryParse(ctorArgs[0], out var len))
                {
                    return (SizeKind.Instance, len);
                }

                return (SizeKind.Instance, null);
            }
        }

        // Fallback: use first ctor arg as size
        if ((ctorArgs.Count > 0) && Int32.TryParse(ctorArgs[0], out var fallback))
        {
            return (SizeKind.Instance, fallback);
        }

        return (SizeKind.Instance, null);
    }

    private static void ResolveLayout(
        List<MemberMappingModel> members,
        List<TypeMappingModel> typeMappings,
        bool validateLayout,
        string typeName,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
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
                {
                    errors.Add(new DiagnosticInfo(Diagnostics.RangeOverlap, syntax.GetLocation(), typeName));
                }
            }
        }
    }

    // -------------------------------------------------------
    // Generator
    // -------------------------------------------------------

    private static void Execute(SourceProductionContext context, ImmutableArray<Result<MapperMethodModel>> results)
    {
        foreach (var error in results.SelectError())
        {
            context.ReportDiagnostic(error);
        }

        var builder = new SourceBuilder();

        var methodsByClass = results.SelectValue()
            .GroupBy(m => new { m.Namespace, m.ClassName });

        foreach (var group in methodsByClass)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            // Report per-method diagnostics
            foreach (var m in group)
            {
                foreach (var err in m.Errors.AsArray())
                {
                    context.ReportDiagnostic(err);
                }
            }

            // Assign method indices within the group
            var methods = group.ToList();
            var numberedMethods = new List<MapperMethodModel>();
            for (var i = 0; i < methods.Count; i++)
            {
                var m = methods[i];
                // Reassign field names with correct method index
                var fixedMembers = m.Members.AsArray().Select((member, pi) =>
                    member with
                    {
                        Converter = member.Converter with { FieldName = $"Converter{i}_{pi}" }
                    }).ToArray();
                numberedMethods.Add(m with
                {
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

        if (!String.IsNullOrEmpty(first.Namespace))
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
            foreach (var member in method.Members.AsArray())
            {
                builder.Indent()
                    .Append("// [")
                    .Append(member.PropertyName)
                    .Append("] offset=")
                    .Append(member.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .NewLine();
                builder.Indent()
                    .Append("private static readonly ")
                    .Append(member.Converter.ConverterTypeFqn)
                    .Append(" ")
                    .Append(member.Converter.FieldName)
                    .Append(" = new(")
                    .Append(String.Join(", ", member.Converter.CtorArgExpressions.AsArray()))
                    .Append(");")
                    .NewLine();
            }
        }

        // Emit methods
        var methodFirst = true;
        foreach (var method in methods)
        {
            if (!methodFirst)
            {
                builder.NewLine();
            }
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
                    .Append("(global::System.ReadOnlySpan<byte> ").Append(method.BufferParamName).Append(", ")
                    .Append(method.TargetTypeFqn).Append(" ").Append(method.TargetParamName).Append(")")
                    .NewLine();
                builder.BeginScope();
                foreach (var member in method.Members.AsArray())
                {
                    EmitReadMember(builder, member, method.BufferParamName, method.TargetParamName);
                }
                builder.EndScope();
                break;

            case MapperShape.NewInstance:
                builder.Indent()
                    .Append(accessibility).Append(" static partial ")
                    .Append(method.TargetTypeFqn).Append(" ")
                    .Append(method.MethodName)
                    .Append("(global::System.ReadOnlySpan<byte> ").Append(method.BufferParamName).Append(")")
                    .NewLine();
                builder.BeginScope();
                builder.Indent()
                    .Append("var ").Append(method.TargetParamName).Append(" = new ").Append(method.TargetTypeFqn).Append("();")
                    .NewLine();
                foreach (var member in method.Members.AsArray())
                {
                    EmitReadMember(builder, member, method.BufferParamName, method.TargetParamName);
                }
                builder.Indent().Append("return ").Append(method.TargetParamName).Append(";").NewLine();
                builder.EndScope();
                break;

            case MapperShape.WriteSpan:
                builder.Indent()
                    .Append(accessibility).Append(" static partial void ")
                    .Append(method.MethodName)
                    .Append("(global::System.Span<byte> ").Append(method.BufferParamName)
                    .Append(", ").Append(method.TargetTypeFqn).Append(" ").Append(method.TargetParamName).Append(")")
                    .NewLine();
                builder.BeginScope();
                EmitWriteBody(builder, method, method.BufferParamName);
                builder.EndScope();
                break;

            case MapperShape.WriteAlloc:
                builder.Indent()
                    .Append(accessibility).Append(" static partial byte[] ")
                    .Append(method.MethodName)
                    .Append("(").Append(method.TargetTypeFqn).Append(" ").Append(method.TargetParamName).Append(")")
                    .NewLine();
                builder.BeginScope();
                builder.Indent()
                    .Append($"var {method.BufferParamName} = new byte[{method.Size}];")
                    .NewLine();
                builder.Indent()
                    .Append($"var span = (global::System.Span<byte>){method.BufferParamName};")
                    .NewLine();
                EmitWriteBody(builder, method, "span");
                builder.Indent().Append("return ").Append(method.BufferParamName).Append(";").NewLine();
                builder.EndScope();
                break;
        }
    }

    private static void EmitReadMember(SourceBuilder builder, MemberMappingModel member, string bufferParam, string targetParam)
    {
        string size;
        if (member.Converter.SizeKind == SizeKind.Const)
        {
            size = $"{member.Converter.ConstSize!.Value}";
        }
        else if (member.Converter.SizeKind == SizeKind.StaticMember)
        {
            size = $"{member.Converter.ConverterTypeFqn}.Size";
        }
        else if (member.Converter.ConstSize.HasValue)
        {
            size = $"{member.Converter.ConstSize.Value}";
        }
        else
        {
            size = $"{member.Converter.FieldName}.Size";
        }

        builder.Indent()
            .Append(targetParam).Append(".")
            .Append(member.PropertyName)
            .Append(" = ")
            .Append(member.Converter.FieldName)
            .Append(".Read(").Append(bufferParam).Append(".Slice(")
            .Append($"{member.Offset}")
            .Append(", ")
            .Append(size)
            .Append("));")
            .NewLine();
    }

    private static void EmitWriteBody(SourceBuilder builder, MapperMethodModel method, string spanVarName = "buffer")
    {
        // Write constants/fillers
        foreach (var tm in method.TypeMappings.AsArray())
        {
            if (tm.Kind == TypeMappingKind.Filler)
            {
                builder.Indent()
                    .Append($"{spanVarName}.Slice({tm.Offset}, {tm.Size}).Fill((byte)0x{tm.Filler:X2});")
                    .NewLine();
            }
            else if (tm.Kind == TypeMappingKind.Constant)
            {
                var bytes = String.Join(", ", tm.Constant.AsArray().Select(b => $"0x{b:X2}"));
                builder.Indent()
                    .Append($"{{ var c = new byte[] {{ {bytes} }}; c.CopyTo({spanVarName}.Slice({tm.Offset}, {tm.Size})); }}")
                    .NewLine();
            }
        }

        foreach (var member in method.Members.AsArray())
        {
            string size;
            if (member.Converter.SizeKind == SizeKind.Const)
            {
                size = $"{member.Converter.ConstSize!.Value}";
            }
            else if (member.Converter.SizeKind == SizeKind.StaticMember)
            {
                size = $"{member.Converter.ConverterTypeFqn}.Size";
            }
            else if (member.Converter.ConstSize.HasValue)
            {
                size = $"{member.Converter.ConstSize.Value}";
            }
            else
            {
                size = $"{member.Converter.FieldName}.Size";
            }

            builder.Indent()
                .Append(member.Converter.FieldName)
                .Append($".Write({spanVarName}.Slice(")
                .Append($"{member.Offset}")
                .Append(", ")
                .Append(size)
                .Append("), ").Append(method.TargetParamName).Append(".")
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
        if (!String.IsNullOrEmpty(ns))
        {
            buffer.Append(ns.Replace('.', '_'));
            buffer.Append('_');
        }
        buffer.Append(className.Replace('<', '[').Replace('>', ']'));
        buffer.Append(".g.cs");
        return buffer.ToString();
    }
}
