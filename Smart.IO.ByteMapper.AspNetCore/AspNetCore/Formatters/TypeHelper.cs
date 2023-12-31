namespace Smart.AspNetCore.Formatters;

internal static class TypeHelper
{
    public static bool IsEnumerableType(Type type)
    {
        return new[] { type }.Where(static x => x.IsInterface).Concat(type.GetInterfaces())
            .Any(static x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static Type GetEnumerableElementType(Type type)
    {
        // Array
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        // IEnumerable type
        if (IsEnumerableType(type))
        {
            return type.GenericTypeArguments[0];
        }

        return null;
    }
}
