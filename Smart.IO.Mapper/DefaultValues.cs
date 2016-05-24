namespace Smart
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///
    /// </summary>
    public static class DefaultValue
    {
        private static readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Of(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType && (Nullable.GetUnderlyingType(type) == null))
            {
                lock (Cache)
                {
                    object value;
                    if (!Cache.TryGetValue(type, out value))
                    {
                        value = Activator.CreateInstance(type);
                        Cache[type] = value;
                    }

                    return value;
                }
            }

            return null;
        }
    }
}
