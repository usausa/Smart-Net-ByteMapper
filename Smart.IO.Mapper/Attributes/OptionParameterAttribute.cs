namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = true)]
    public class OptionParameterAttribute : Attribute, IOptionParameterAttribute
    {
        public string Key { get; }

        public object Value { get; }

        public OptionParameterAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
