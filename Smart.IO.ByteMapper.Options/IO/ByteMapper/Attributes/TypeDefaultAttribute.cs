namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeZeroFillAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => OptionsParameter.ZeroFill;

        public object Value { get; }

        public TypeZeroFillAttribute(bool value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeUseGroupingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => OptionsParameter.UseGrouping;

        public object Value { get; }

        public TypeUseGroupingAttribute(bool value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberPaddingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => OptionsParameter.NumberPadding;

        public object Value { get; }

        public TypeNumberPaddingAttribute(Padding value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => OptionsParameter.NumberFiller;

        public object Value { get; }

        public TypeNumberFillerAttribute(byte value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeUnicodeFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => OptionsParameter.UnicodeFiller;

        public object Value { get; }

        public TypeUnicodeFillerAttribute(char value)
        {
            Value = value;
        }
    }
}
