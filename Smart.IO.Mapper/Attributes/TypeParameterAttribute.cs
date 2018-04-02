namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class TypeParameterAttribute : Attribute, ITypeParameterAttribute
    {
        public string Key { get; }

        public object Value { get; }

        public TypeParameterAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
