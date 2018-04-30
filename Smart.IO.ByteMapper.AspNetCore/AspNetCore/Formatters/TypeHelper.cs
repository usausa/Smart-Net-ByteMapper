namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeHelper
    {
        private static readonly Type EnumerableType = typeof(IEnumerable<>);

        public static bool IsEnumerableType(Type type)
        {
            return new[] { type }.Where(x => x.IsInterface).Concat(type.GetInterfaces())
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == EnumerableType);
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
}
