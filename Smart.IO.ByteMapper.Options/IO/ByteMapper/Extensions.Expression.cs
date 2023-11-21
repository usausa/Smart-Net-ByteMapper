namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Expressions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "Ignore")]
public static class OptionsExpressionExtensions
{
    //--------------------------------------------------------------------------------
    // Type
    //--------------------------------------------------------------------------------

    // Default

    public static ITypeConfigSyntax<T> TypeNumberPadding<T>(this ITypeConfigSyntax<T> syntax, Padding value)
    {
        return syntax.TypeDefault(OptionsParameter.NumberPadding, value);
    }

    public static ITypeConfigSyntax<T> TypeZeroFill<T>(this ITypeConfigSyntax<T> syntax, bool value)
    {
        return syntax.TypeDefault(OptionsParameter.ZeroFill, value);
    }

    public static ITypeConfigSyntax<T> TypeUseGrouping<T>(this ITypeConfigSyntax<T> syntax, bool value)
    {
        return syntax.TypeDefault(OptionsParameter.UseGrouping, value);
    }

    public static ITypeConfigSyntax<T> TypeNumberFiller<T>(this ITypeConfigSyntax<T> syntax, byte value)
    {
        return syntax.TypeDefault(OptionsParameter.NumberFiller, value);
    }

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    // Ascii

    public static IMapAsciiSyntax Ascii(this IMemberMapConfigSyntax syntax, int length)
    {
        var expression = new MapAsciiExpression(length);
        syntax.Map(expression);
        return expression;
    }

    // Unicode

    public static IMapUnicodeSyntax Unicode(this IMemberMapConfigSyntax syntax, int length)
    {
        var expression = new MapUnicodeExpression(length);
        syntax.Map(expression);
        return expression;
    }

    // Integer

    public static IMapIntegerSyntax Integer(this IMemberMapConfigSyntax syntax, int length)
    {
        var expression = new MapIntegerExpression(length);
        syntax.Map(expression);
        return expression;
    }

    // Decimal

    public static IMapDecimalSyntax Decimal(this IMemberMapConfigSyntax syntax, int length)
    {
        var expression = new MapDecimalExpression(length);
        syntax.Map(expression);
        return expression;
    }

    public static IMapDecimalSyntax Decimal(this IMemberMapConfigSyntax syntax, int length, byte scale)
    {
        var expression = new MapDecimalExpression(length, scale);
        syntax.Map(expression);
        return expression;
    }

    // DateTime

    public static IMapDateTimeSyntax DateTime(this IMemberMapConfigSyntax syntax, string format)
    {
        var expression = new MapDateTimeExpression(format);
        syntax.Map(expression);
        return expression;
    }

    public static IMapDateTimeSyntax DateTime(this IMemberMapConfigSyntax syntax, string format, DateTimeKind kind)
    {
        var expression = new MapDateTimeExpression(format, kind);
        syntax.Map(expression);
        return expression;
    }
}
