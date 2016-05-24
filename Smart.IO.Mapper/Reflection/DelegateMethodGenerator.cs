namespace Smart.Reflection
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///
    /// </summary>
    public static class DelegateMethodGenerator
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Delegate CreateTypedGetDelegate(PropertyInfo pi)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }

            if (pi.DeclaringType == null)
            {
                throw new ArgumentException("DeclaringType is null type.", pi.Name);
            }

            if (pi.DeclaringType.IsValueType)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Value type is not supported type {0}.", pi.DeclaringType));
            }

            if (pi.GetIndexParameters().Any())
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Property has index parameters {0}.", pi.Name));
            }

            var getter = pi.GetGetMethod(true);
            if (getter == null)
            {
                return null;
            }

            var getterDelegateType = typeof(Func<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
            return Delegate.CreateDelegate(getterDelegateType, null, getter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Func<TTarget, TMember> CreateTypedGetDelegate<TTarget, TMember>(PropertyInfo pi)
            where TTarget : class
        {
            return (Func<TTarget, TMember>)CreateTypedGetDelegate(pi);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Delegate CreateTypedSetDelegate(PropertyInfo pi)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }

            if (pi.DeclaringType == null)
            {
                throw new ArgumentException("DeclaringType is null type.", pi.Name);
            }

            if (pi.DeclaringType.IsValueType)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Value type is not supported type {0}.", pi.DeclaringType));
            }

            if (pi.GetIndexParameters().Any())
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Property has index parameters {0}.", pi.Name));
            }

            var setter = pi.GetSetMethod(true);
            if (setter == null)
            {
                return null;
            }

            var setterDelegateType = typeof(Action<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
            return Delegate.CreateDelegate(setterDelegateType, null, setter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Action<TTarget, TMember> CreateTypedSetDelegate<TTarget, TMember>(PropertyInfo pi)
            where TTarget : class
        {
            return (Action<TTarget, TMember>)CreateTypedSetDelegate(pi);
        }
    }
}
