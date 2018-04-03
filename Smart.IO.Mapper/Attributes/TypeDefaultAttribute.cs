﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class TypeDefaultAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key { get; }

        public object Value { get; }

        public TypeDefaultAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}