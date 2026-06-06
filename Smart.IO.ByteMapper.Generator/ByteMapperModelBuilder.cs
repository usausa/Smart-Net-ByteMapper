namespace Smart.IO.ByteMapper.Generator;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Smart.IO.ByteMapper.Generator.Helpers;
using Smart.IO.ByteMapper.Generator.Models;

using SourceGenerateHelper;

// Parse/transform stage: converts Roslyn symbols into the equatable MapperMethodModel.
// Pure of any source emission; all diagnostics are collected here so the source builder can
// assume a valid model.
internal static class ByteMapperModelBuilder
{
    internal const string ByteReaderAttributeName = "Smart.IO.ByteMapper.ByteReaderAttribute";
    internal const string ByteWriterAttributeName = "Smart.IO.ByteMapper.ByteWriterAttribute";
    private const string MapAttributeName = "Smart.IO.ByteMapper.MapAttribute";
    private const string MapProfileAttributeName = "Smart.IO.ByteMapper.MapProfileAttribute";
    private const string MapFillerAttributeName = "Smart.IO.ByteMapper.MapFillerAttribute";
    private const string MapConstantAttributeName = "Smart.IO.ByteMapper.MapConstantAttribute";
    private const string ByteMapperPropertyAttributeOpenName = "Smart.IO.ByteMapper.ByteMapperPropertyAttribute`1";
    private const string ConverterSupportedTypesAttributeName = "Smart.IO.ByteMapper.ConverterSupportedTypesAttribute";

    public static Result<MapperMethodModel> Parse(GeneratorAttributeSyntaxContext context, MapperKind kind)
    {
        var syntax = (MethodDeclarationSyntax)context.TargetNode;
        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not IMethodSymbol symbol)
        {
            return Results.Errors<MapperMethodModel>();
        }

        if (!symbol.IsStatic || !symbol.IsPartialDefinition)
        {
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodDefinition, syntax.GetLocation(), symbol.Name));
        }

        // Determine shape and target type / メソッドシグネチャからマッパーの形状とターゲット型を決定する
        var (shape, targetType, bufferParamName, targetParamName, errors) = DetermineShape(symbol, kind);
        if (errors.Count > 0)
        {
            return Results.Error<MapperMethodModel>(errors[0]);
        }

        if (targetType is null)
        {
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidMethodSignature, syntax.GetLocation(), symbol.Name));
        }

        // Check for SBM0014: return-value reader requires parameterless constructor
        // SBM0014 チェック: 戻り値型リーダーはデフォルトコンストラクターが必要
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

        // Get ByteReader/Writer attribute / ByteReader/Writer 属性を取得する
        var methodAttr = symbol.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == (kind == MapperKind.Reader ? ByteReaderAttributeName : ByteWriterAttributeName));

        ITypeSymbol? profileType = null;
        var validateLayout = true;
        if (methodAttr is not null)
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

        // Determine attribute source type (Profile or Target) / 属性ソース型を決定する（Profile 指定があれば Profile 型、なければターゲット型）
        var attrSourceType = profileType ?? targetType;

        // Get [Map] / [MapProfile] from attribute source / 属性ソース型から [Map] / [MapProfile] を取得する
        // [Map] = object self-layout described by property attributes.
        // [MapProfile] = profile layout described by class-level [Map...Member] attributes.
        // [Map] = プロパティ属性で記述するオブジェクト自身のレイアウト。
        // [MapProfile] = クラスレベルの [Map...Member] 属性で記述するプロファイルのレイアウト。
        var mapAttr = attrSourceType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapAttributeName);
        var mapProfileAttr = attrSourceType.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapProfileAttributeName);

        // SBM0017: [Map] and [MapProfile] are mutually exclusive / [Map] と [MapProfile] は併用不可
        if (mapAttr is not null && mapProfileAttr is not null)
        {
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.ConflictingMapAttributes, syntax.GetLocation(), attrSourceType.Name));
        }

        var effectiveMapAttr = mapProfileAttr ?? mapAttr;
        if (effectiveMapAttr is null)
        {
            var diagId = profileType is not null ? Diagnostics.ProfileMissingMapAttribute : Diagnostics.MissingMapAttribute;
            return Results.Error<MapperMethodModel>(new DiagnosticInfo(diagId, syntax.GetLocation(), symbol.Name));
        }

        // Profile mode collects member mappings from class-level [Map...Member] attributes;
        // object mode collects them from property attributes (the original behavior).
        // プロファイルモードはクラスレベルの [Map...Member] 属性から、オブジェクトモードはプロパティ属性から
        // メンバーマッピングを収集する（後者が従来の挙動）。
        var isProfileMode = mapProfileAttr is not null;
        var mapSize = (int)(effectiveMapAttr.ConstructorArguments[0].Value ?? 0);

        // Parse optional Map settings / Map 属性のオプション設定を解析する
        var autoFiller = true;
        byte? nullFiller = null;
        var useDelimiter = true;
        byte[]? delimiter = null;
        foreach (var na in effectiveMapAttr.NamedArguments)
        {
            switch (na.Key)
            {
                case "AutoFiller" when na.Value.Value is bool af:
                    autoFiller = af;
                    break;
                case "UseDelimiter" when na.Value.Value is bool ud:
                    useDelimiter = ud;
                    break;
                case "NullFiller" when na.Value.Value is byte nf:
                    nullFiller = nf;
                    break;
                case "Delimiter" when na.Value.Kind == TypedConstantKind.Array:
                    var delimBytes = na.Value.Values
                        .Select(static v => v.Value is byte b ? b : (byte)0)
                        .ToArray();
                    if (delimBytes.Length > 0)
                    {
                        delimiter = delimBytes;
                    }
                    break;
            }
        }

        var containingType = symbol.ContainingType;
        var ns = String.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        // Collect fillers and constants from type attributes / 型属性からフィラーと定数マッピングを収集する
        var typeMappings = CollectTypeMappings(attrSourceType);

        // Delimiter: written as constant at end of record / 区切り文字をレコード末尾の定数マッピングとして追加する
        if (useDelimiter && delimiter is { Length: > 0 })
        {
            // SBM0004: a delimiter longer than the record yields a negative offset / 区切り文字がレコード長を超えると負のオフセットになる
            if (delimiter.Length > mapSize)
            {
                return Results.Error<MapperMethodModel>(new DiagnosticInfo(Diagnostics.InvalidOffset, syntax.GetLocation(), $"{symbol.Name}, Delimiter"));
            }
            typeMappings.Add(new TypeMappingModel(mapSize - delimiter.Length, delimiter.Length, TypeMappingKind.Constant, new EquatableArray<byte>(delimiter), 0));
        }

        // Collect member mappings / メンバーマッピングを収集する
        var compilation = context.SemanticModel.Compilation;
        var propertyAttrBase = compilation.GetTypeByMetadataName(ByteMapperPropertyAttributeOpenName);

        if (propertyAttrBase is null)
        {
            return Results.Errors<MapperMethodModel>();
        }

        var diagnostics = new List<DiagnosticInfo>();
        List<MemberMappingModel> members;
        if (isProfileMode)
        {
            members = CollectMemberMappings(symbol, attrSourceType, targetType, propertyAttrBase, syntax, diagnostics);

            // SBM0016: property-level mapping attributes are ignored under [MapProfile] / [MapProfile] 下ではプロパティのマッピング属性は無視される
            if (HasPropertyMappingAttribute(attrSourceType, propertyAttrBase))
            {
                diagnostics.Add(new DiagnosticInfo(Diagnostics.PropertyMappingIgnoredUnderProfile, syntax.GetLocation(), attrSourceType.Name));
            }
        }
        else
        {
            members = CollectMembers(symbol, attrSourceType, targetType, profileType, propertyAttrBase, syntax, diagnostics);

            // SBM0015: class-level [Map...Member] attributes are ignored under [Map] / [Map] 下ではクラスレベルの [Map...Member] 属性は無視される
            if (HasMemberMappingAttribute(attrSourceType, propertyAttrBase))
            {
                diagnostics.Add(new DiagnosticInfo(Diagnostics.MemberAttributeRequiresProfile, syntax.GetLocation(), attrSourceType.Name));
            }
        }

        // Layout resolution / レイアウトの解決（オフセット順ソート＆重複チェック＆サイズ超過チェック）
        ResolveLayout(
            members,
            typeMappings,
            validateLayout,
            attrSourceType.Name,
            symbol.Name,
            mapSize,
            syntax,
            diagnostics);

        // Auto-fill uncovered gaps in the write path / NullFiller が設定されている場合のみギャップを自動フィルする
        if (autoFiller && nullFiller.HasValue)
        {
            ApplyAutoFill(members, typeMappings, mapSize, nullFiller.Value);
        }

        var className = containingType.GetClassName();
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
            new EquatableArray<DiagnosticInfo>(diagnostics.ToArray()));

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

    // Object mode: walk the properties of the attribute source type and read the converter
    // attribute hung on each property (the original behavior).
    // オブジェクトモード: 属性ソース型のプロパティを走査し、各プロパティに付いたコンバーター属性を読む（従来の挙動）。
    private static List<MemberMappingModel> CollectMembers(
        IMethodSymbol methodSymbol,
        ITypeSymbol attrSourceType,
        ITypeSymbol targetType,
        ITypeSymbol? profileType,
        INamedTypeSymbol propertyAttrBase,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        var members = new List<MemberMappingModel>();
        var propertyIndex = 0;

        // Walk properties in attribute source type / 属性ソース型のプロパティを順に走査する
        foreach (var member in attrSourceType.GetMembers().OfType<IPropertySymbol>())
        {
            foreach (var attr in member.GetAttributes())
            {
                // ReSharper disable UseNullPropagation
                if (attr.AttributeClass is null)
                {
                    continue;
                }

                // Try to get Converter type from ByteMapperPropertyAttribute<TConverter>
                // ByteMapperPropertyAttribute<TConverter> からコンバーター型を取得する
                var converterBase = attr.AttributeClass.FindConverterAttributeBase(propertyAttrBase);
                if (converterBase is null)
                {
                    continue; // unrecognized converter attribute - skip
                }

                var offset = (int)(attr.ConstructorArguments[0].Value ?? 0);

                // Determine actual property symbol on target / ターゲット型上の実プロパティシンボルを特定する
                var targetProp = member;
                if (profileType is not null)
                {
                    var found = targetType.GetMembers(member.Name).OfType<IPropertySymbol>().FirstOrDefault();
                    if (found is null)
                    {
                        errors.Add(new DiagnosticInfo(Diagnostics.ProfilePropertyNotFound, syntax.GetLocation(), $"{methodSymbol.Name}, {member.Name}"));
                        continue;
                    }
                    targetProp = found;
                }

                // Property attribute layout: ctorArgs[0] is offset / プロパティ属性のレイアウト: ctorArgs[0] はオフセット
                var mapping = BuildMemberMapping(methodSymbol, attr, attr.AttributeClass, converterBase, targetProp, offset, leadingCtorArgs: 1, propertyIndex, syntax, errors);
                if (mapping is not null)
                {
                    members.Add(mapping);
                    propertyIndex++;
                }

                break; // Only first attribute per property / プロパティごとに最初の属性のみ処理する
            }
        }

        return members;
    }

    // Profile mode: walk the class-level [Map...Member] attributes on the profile and resolve each
    // target property by the member name carried in the attribute (ctorArgs[0]).
    // プロファイルモード: プロファイルのクラスレベル [Map...Member] 属性を走査し、属性が持つメンバー名
    // (ctorArgs[0]) で対象プロパティを引き当てる。
    private static List<MemberMappingModel> CollectMemberMappings(
        IMethodSymbol methodSymbol,
        ITypeSymbol attrSourceType,
        ITypeSymbol targetType,
        INamedTypeSymbol propertyAttrBase,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        var members = new List<MemberMappingModel>();
        var propertyIndex = 0;

        foreach (var attr in attrSourceType.GetAttributes())
        {
            if (attr.AttributeClass is not { } attrClass)
            {
                continue;
            }

            // Only attributes deriving from ByteMapperPropertyAttribute<TConverter> are member mappings.
            // ByteMapperPropertyAttribute<TConverter> 派生の属性のみがメンバーマッピング。
            var converterBase = attrClass.FindConverterAttributeBase(propertyAttrBase);
            if (converterBase is null)
            {
                continue; // [MapProfile] / [MapFiller] / [MapConstant] etc. - skip
            }

            // Member attribute layout: ctorArgs[0] is member name, ctorArgs[1] is offset
            // メンバー属性のレイアウト: ctorArgs[0] はメンバー名、ctorArgs[1] はオフセット
            if (attr.ConstructorArguments.Length < 2)
            {
                continue;
            }

            var memberName = attr.ConstructorArguments[0].Value as string;
            if (String.IsNullOrEmpty(memberName))
            {
                continue;
            }

            var offset = (int)(attr.ConstructorArguments[1].Value ?? 0);

            var targetProp = targetType.GetMembers(memberName!).OfType<IPropertySymbol>().FirstOrDefault();
            if (targetProp is null)
            {
                errors.Add(new DiagnosticInfo(Diagnostics.ProfilePropertyNotFound, syntax.GetLocation(), $"{methodSymbol.Name}, {memberName}"));
                continue;
            }

            var mapping = BuildMemberMapping(methodSymbol, attr, attrClass, converterBase, targetProp, offset, leadingCtorArgs: 2, propertyIndex, syntax, errors);
            if (mapping is not null)
            {
                members.Add(mapping);
                propertyIndex++;
            }
        }

        return members;
    }

    // Builds a single member mapping shared by both modes. Returns null (after adding a diagnostic)
    // when the converter does not satisfy the type/contract checks.
    // 両モードで共有する単一メンバーマッピングの組み立て。型/契約チェックを満たさない場合は
    // 診断を追加して null を返す。
    private static MemberMappingModel? BuildMemberMapping(
        IMethodSymbol methodSymbol,
        AttributeData attr,
        INamedTypeSymbol attrClass,
        INamedTypeSymbol converterBase,
        IPropertySymbol targetProp,
        int offset,
        int leadingCtorArgs,
        int propertyIndex,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        var converterType = converterBase.TypeArguments[0];
        var converterFqn = converterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // SBM0008: check [ConverterSupportedTypes] on the attribute class
        // SBM0008 チェック: 属性クラスの [ConverterSupportedTypes] でプロパティ型が許可されているか検証する
        if (!CheckSupportedTypes(attrClass, targetProp.Type, syntax, methodSymbol, targetProp, errors))
        {
            return null;
        }

        // SBM0010: converter Read/Write must be instance methods / コンバーターの Read/Write はインスタンスメソッドである必要がある
        // The source builder always emits instance calls (field.Read / field.Write), so a static
        // Read or Write would break with a plain compile error rather than a diagnostic.
        var namedConverterType = converterType as INamedTypeSymbol;
        var readMethod = namedConverterType?.GetMembers("Read").OfType<IMethodSymbol>().FirstOrDefault();
        var writeMethod = namedConverterType?.GetMembers("Write").OfType<IMethodSymbol>().FirstOrDefault();
        if (readMethod?.IsStatic == true || writeMethod?.IsStatic == true)
        {
            errors.Add(new DiagnosticInfo(Diagnostics.ConverterContractMismatch, syntax.GetLocation(), $"{methodSymbol.Name}, {targetProp.Name}"));
            return null;
        }

        // Build ctor arg expressions / コンストラクター引数式を構築する
        var ctorArgs = BuildConverterCtorArgs(attr, converterType, leadingCtorArgs);

        // Determine size / コンバーターのサイズ種別と定数サイズを決定する
        var (sizeKind, constSize) = DetermineConverterSize(converterType, ctorArgs);

        var fieldName = $"Converter0_{propertyIndex}"; // MethodIndex will be fixed in MapperSourceBuilder / メソッドインデックスはソースビルダーで確定される
        var converterCall = new ConverterCallModel(
            converterFqn,
            fieldName,
            new EquatableArray<string>(ctorArgs.ToArray()),
            sizeKind,
            constSize);

        var size = constSize ?? 0; // will be fixed for instance size converters

        return new MemberMappingModel(targetProp.Name, offset, size, converterCall);
    }

    // True when the type carries any class-level [Map...Member] attribute (used to warn under [Map]).
    // 型にクラスレベルの [Map...Member] 属性があるか（[Map] 下での警告に使用）。
    private static bool HasMemberMappingAttribute(ITypeSymbol type, INamedTypeSymbol propertyAttrBase) =>
        type.GetAttributes().Any(a => a.AttributeClass?.FindConverterAttributeBase(propertyAttrBase) is not null);

    // True when any property of the type carries a converter attribute (used to warn under [MapProfile]).
    // 型のいずれかのプロパティにコンバーター属性があるか（[MapProfile] 下での警告に使用）。
    private static bool HasPropertyMappingAttribute(ITypeSymbol type, INamedTypeSymbol propertyAttrBase) =>
        type.GetMembers().OfType<IPropertySymbol>()
            .Any(p => p.GetAttributes().Any(a => a.AttributeClass?.FindConverterAttributeBase(propertyAttrBase) is not null));

    // Checks [ConverterSupportedTypes] on the attribute class against the target property type.
    // Returns false (and adds SBM0008) when the type is not supported.
    // 属性クラスの [ConverterSupportedTypes] をターゲットプロパティ型と照合する。
    // 型が非対応の場合は false を返し SBM0008 を追加する。
    private static bool CheckSupportedTypes(
        INamedTypeSymbol attrClass,
        ITypeSymbol propType,
        MethodDeclarationSyntax syntax,
        IMethodSymbol methodSymbol,
        IPropertySymbol prop,
        List<DiagnosticInfo> errors)
    {
        // [ConverterSupportedTypes] lives on the abstract base shared by the property and member forms,
        // so walk the base chain to find it. / [ConverterSupportedTypes] はプロパティ版とメンバー版が共有する
        // 抽象ベースに付くため、基底チェーンを辿って探す。
        AttributeData? supportedAttr = null;
        for (var current = (INamedTypeSymbol?)attrClass; current is not null && supportedAttr is null; current = current.BaseType)
        {
            supportedAttr = current.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ConverterSupportedTypesAttributeName);
        }

        if (supportedAttr is null)
        {
            // No restriction declared — always allowed / 制限なし — 常に許可
            return true;
        }

        // Fixed type list: property type must be in Types[] / 固定型リスト: プロパティ型が Types[] に含まれている必要がある
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

    // leadingCtorArgs is the number of leading attribute ctor args that are not converter positional args:
    // 1 for property attributes (offset), 2 for member attributes (member name, offset).
    // leadingCtorArgs はコンバーターの位置引数ではない先頭の属性コンストラクター引数の数:
    // プロパティ属性は 1（オフセット）、メンバー属性は 2（メンバー名・オフセット）。
    private static List<string> BuildConverterCtorArgs(
        AttributeData attr,
        ITypeSymbol converterType,
        int leadingCtorArgs)
    {
        // Collect ctor args from attribute: skip the leading non-converter args / 属性からコンストラクター引数を収集する（先頭の非コンバーター引数はスキップ）
        var args = new List<string>();
        for (var i = leadingCtorArgs; i < attr.ConstructorArguments.Length; i++)
        {
            args.Add(attr.ConstructorArguments[i].ToLiteralExpression());
        }

        // Build lookup of named arguments (user-specified) / ユーザー指定の名前付き引数のルックアップを構築する
        var namedArgs = attr.NamedArguments.ToDictionary(na => na.Key, na => na.Value.ToLiteralExpression());

        // Build lookup of attribute property defaults from attribute class property initializers
        // 属性クラスのプロパティイニシャライザーからデフォルト値のルックアップを構築する
        var attrPropDefaults = GetAttributePropertyDefaults(attr.AttributeClass);

        if (converterType is INamedTypeSymbol namedConverter)
        {
            var ctor = namedConverter.InstanceConstructors.FirstOrDefault(c => c.DeclaredAccessibility == Accessibility.Public);
            if (ctor is not null)
            {
                // Map converter ctor params (skip already-covered positional params)
                // コンバーターのコンストラクターパラメーターをマップする（位置引数で既に埋まっているものはスキップ）
                // The leading args (offset, and member name for member attributes) are skipped above, so the
                // remaining attribute ctor args already fill the first converter params.
                // 先頭引数（オフセット、メンバー属性ではメンバー名）は上でスキップ済みのため、残りの属性
                // コンストラクター引数が先頭のコンバーターパラメーターを埋めている。
                var coveredCount = attr.ConstructorArguments.Length - leadingCtorArgs; // number of converter ctor params already filled / 埋め済みのコンバーターコンストラクターパラメーター数
                for (var i = coveredCount; i < ctor.Parameters.Length; i++)
                {
                    var param = ctor.Parameters[i];
                    var pascalName = param.Name.Length > 0
                        ? param.Name.Substring(0, 1).ToUpperInvariant() + param.Name.Substring(1)
                        : param.Name;

                    // 1) User-specified named argument (pascal or camel) / ユーザー指定の名前付き引数（パスカルケースまたはキャメルケース）
                    if (namedArgs.TryGetValue(pascalName, out var val) || namedArgs.TryGetValue(param.Name, out val))
                    {
                        args.Add(val);
                        continue;
                    }

                    // 2) Attribute property default value / 属性プロパティのデフォルト値
                    if (attrPropDefaults.TryGetValue(pascalName, out val))
                    {
                        args.Add(val);
                        continue;
                    }

                    // 3) Converter ctor parameter explicit default / コンバーターコンストラクターパラメーターの明示的デフォルト値
                    args.Add(GetDefaultLiteral(param));
                }
            }
        }

        return args;
    }

    // Reads the default values from attribute class property initializers.
    // Returns pascal-cased property name → C# literal expression.
    // The options are declared on the abstract base shared by the property and member forms, so walk the
    // base chain; a derived declaration wins over a base one.
    // 属性クラスのプロパティイニシャライザーからデフォルト値を読み取る。
    // パスカルケースのプロパティ名 → C# リテラル式 の辞書を返す。
    // オプションはプロパティ版とメンバー版が共有する抽象ベースに宣言されるため基底チェーンを辿る。
    // 派生側の宣言が基底側より優先される。
    private static Dictionary<string, string> GetAttributePropertyDefaults(INamedTypeSymbol? attrClass)
    {
        var result = new Dictionary<string, string>();

        for (var current = attrClass; current is not null; current = current.BaseType)
        {
            foreach (var member in current.GetMembers())
            {
                if (member is not IPropertySymbol prop)
                {
                    continue;
                }

                if (prop.IsStatic || (prop.DeclaredAccessibility != Accessibility.Public))
                {
                    continue;
                }

                if (result.ContainsKey(prop.Name))
                {
                    continue; // derived declaration already recorded / 派生側の宣言を記録済み
                }

                foreach (var syntaxRef in prop.DeclaringSyntaxReferences)
                {
                    var node = syntaxRef.GetSyntax();
                    if (node is PropertyDeclarationSyntax propSyntax
                        && propSyntax.Initializer?.Value is not null)
                    {
                        result[prop.Name] = propSyntax.Initializer.Value.ToString();
                        break;
                    }
                }
            }
        }

        return result;
    }

    private static string GetDefaultLiteral(IParameterSymbol param)
    {
        if (param.HasExplicitDefaultValue)
        {
            if (param.Type.TypeKind == TypeKind.Enum && param.ExplicitDefaultValue is not null)
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
                char c => $"'{c}'",
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
        // const の Size フィールドまたは static readonly の Size フィールドを確認する
        foreach (var member in namedConverter.GetMembers("Size"))
        {
            if (member is IFieldSymbol field)
            {
                if (field.IsConst && field.HasConstantValue)
                {
                    return (SizeKind.Const, Convert.ToInt32(field.ConstantValue, System.Globalization.CultureInfo.InvariantCulture));
                }

                // static readonly int Size = Unsafe.SizeOf<T>() - value not known at compile time
                // static readonly int Size = Unsafe.SizeOf<T>() — コンパイル時には値が不明
                // For generic converters (e.g. BinaryConverter<int>), resolve the type argument size if possible
                // ジェネリックコンバーター（例: BinaryConverter<int>）の場合、型引数のサイズをヘルパーで解決する
                if (field.IsStatic && field.IsReadOnly)
                {
                    if (namedConverter.IsGenericType && namedConverter.TypeArguments.Length == 1)
                    {
                        var typeArg = namedConverter.TypeArguments[0];
                        if (typeArg.TryGetUnmanagedSize(out var knownSize))
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
                // インスタンス Size プロパティ — コンストラクター引数の先頭（長さ）から値を推定する
                if ((ctorArgs.Count > 0) && Int32.TryParse(ctorArgs[0], out var len))
                {
                    return (SizeKind.Instance, len);
                }

                return (SizeKind.Instance, null);
            }
        }

        // Fallback: use first ctor arg as size / フォールバック: コンストラクター引数の先頭をサイズとして使用する
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
        string methodName,
        int mapSize,
        MethodDeclarationSyntax syntax,
        List<DiagnosticInfo> errors)
    {
        // Sort members by offset / メンバーをオフセット順にソートする
        members.Sort((a, b) => a.Offset.CompareTo(b.Offset));
        typeMappings.Sort((a, b) => a.Offset.CompareTo(b.Offset));

        var allRanges = members.Select(m => (m.Offset, m.Size))
            .Concat(typeMappings.Select(t => (t.Offset, t.Size)))
            .OrderBy(r => r.Offset)
            .ToList();

        // SBM0004: Validate no negative offset or length / 負のオフセット・長さを検証する
        foreach (var (offset, size) in allRanges)
        {
            if (offset < 0 || size < 0)
            {
                errors.Add(new DiagnosticInfo(Diagnostics.InvalidOffset, syntax.GetLocation(), $"{methodName}, {typeName}"));
                break;
            }
        }

        // SBM0006: Validate overlap / 範囲の重複を検証する
        if (validateLayout)
        {
            for (var i = 0; i < allRanges.Count - 1; i++)
            {
                var end = allRanges[i].Offset + allRanges[i].Size;
                if (end > allRanges[i + 1].Offset)
                {
                    errors.Add(new DiagnosticInfo(Diagnostics.RangeOverlap, syntax.GetLocation(), typeName));
                }
            }
        }

        // SBM0007: Validate that no mapping exceeds Map(size) / Map(size) を超えるマッピングがないか検証する
        if (allRanges.Count > 0)
        {
            var maxEnd = allRanges.Max(static r => r.Offset + r.Size);
            if (maxEnd > mapSize)
            {
                errors.Add(new DiagnosticInfo(Diagnostics.LayoutExceedsSize, syntax.GetLocation(), typeName));
            }
        }
    }

    // ReSharper disable once ParameterTypeCanBeEnumerable.Local
    private static void ApplyAutoFill(
        List<MemberMappingModel> members,
        List<TypeMappingModel> typeMappings,
        int mapSize,
        byte fillerByte)
    {
        var covered = members.Select(m => (Start: m.Offset, End: m.Offset + m.Size))
            .Concat(typeMappings.Select(t => (Start: t.Offset, End: t.Offset + t.Size)))
            .OrderBy(static r => r.Start)
            .ToList();

        var fills = new List<TypeMappingModel>();
        var pos = 0;
        foreach (var (start, end) in covered)
        {
            if (pos < start)
            {
                fills.Add(new TypeMappingModel(pos, start - pos, TypeMappingKind.Filler, new EquatableArray<byte>([]), fillerByte));
            }
            if (end > pos)
            {
                pos = end;
            }
        }
        if (pos < mapSize)
        {
            fills.Add(new TypeMappingModel(pos, mapSize - pos, TypeMappingKind.Filler, new EquatableArray<byte>([]), fillerByte));
        }

        if (fills.Count > 0)
        {
            typeMappings.AddRange(fills);
            typeMappings.Sort(static (a, b) => a.Offset.CompareTo(b.Offset));
        }
    }
}
