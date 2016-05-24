namespace Smart.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    ///
    /// </summary>
    public interface IAccessor
    {
        MemberInfo MemberInfo { get; }

        Type Type { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        object GetValue(object target);

        void SetValue(object target, object value);
    }
}
