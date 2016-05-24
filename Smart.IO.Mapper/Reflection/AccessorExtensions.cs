namespace Smart.Reflection
{
    using System;
    using System.Reflection;

    public static class AccessorExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static IAccessor ToAccessor(this PropertyInfo pi)
        {
            var getter = DelegateMethodGenerator.CreateTypedGetDelegate(pi);
            var setter = DelegateMethodGenerator.CreateTypedSetDelegate(pi);

            if (pi.PropertyType.IsValueType)
            {
                var accessorType = typeof(ValueTypeDelegateAccsessor<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
                return (IAccessor)Activator.CreateInstance(accessorType, pi, pi.PropertyType, getter, setter, DefaultValue.Of(pi.PropertyType));
            }
            else
            {
                var accessorType = typeof(DelegateAccsessor<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
                return (IAccessor)Activator.CreateInstance(accessorType, pi, pi.PropertyType, getter, setter);
            }
        }
    }
}
