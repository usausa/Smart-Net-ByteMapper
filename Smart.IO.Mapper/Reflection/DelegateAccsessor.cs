namespace Smart.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    internal class DelegateAccsessor<TTarget, TMember> : IAccessor
    {
        private readonly Func<TTarget, TMember> getter;

        private readonly Action<TTarget, TMember> setter;

        /// <summary>
        ///
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public bool CanRead
        {
            get { return getter != null; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool CanWrite
        {
            get { return setter != null; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="type"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public DelegateAccsessor(MemberInfo memberInfo, Type type, Func<TTarget, TMember> getter, Action<TTarget, TMember> setter)
        {
            MemberInfo = memberInfo;
            Type = type;
            this.getter = getter;
            this.setter = setter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public object GetValue(object target)
        {
            return getter((TTarget)target);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue(object target, object value)
        {
            setter((TTarget)target, (TMember)value);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    internal class ValueTypeDelegateAccsessor<TTarget, TMember> : IAccessor
    {
        private readonly Func<TTarget, TMember> getter;

        private readonly Action<TTarget, TMember> setter;

        private readonly TMember nullValue;

        /// <summary>
        ///
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public bool CanRead
        {
            get { return getter != null; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool CanWrite
        {
            get { return setter != null; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="type"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="nullValue"></param>
        public ValueTypeDelegateAccsessor(MemberInfo memberInfo, Type type, Func<TTarget, TMember> getter, Action<TTarget, TMember> setter, TMember nullValue)
        {
            MemberInfo = memberInfo;
            Type = type;
            this.getter = getter;
            this.setter = setter;
            this.nullValue = nullValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public object GetValue(object target)
        {
            return getter((TTarget)target);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue(object target, object value)
        {
            if (value == null)
            {
                setter((TTarget)target, nullValue);
            }
            else
            {
                setter((TTarget)target, (TMember)value);
            }
        }
    }
}
