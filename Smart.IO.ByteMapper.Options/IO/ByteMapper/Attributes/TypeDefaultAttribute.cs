namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeTextEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => DateTimeTextParameter.Encoding;

        public object Value { get; }

        public TypeDateTimeTextEncodingAttribute(int codePage)
        {
            Value = Encoding.GetEncoding(codePage);
        }

        public TypeDateTimeTextEncodingAttribute(string encodingName)
        {
            Value = Encoding.GetEncoding(encodingName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeTextProviderAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => DateTimeTextParameter.Provider;

        public object Value { get; }

        public TypeDateTimeTextProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeTextStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => DateTimeTextParameter.Style;

        public object Value { get; }

        public TypeDateTimeTextStyleAttribute(DateTimeStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.Encoding;

        public object Value { get; }

        public TypeNumberTextEncodingAttribute(int codePage)
        {
            Value = Encoding.GetEncoding(codePage);
        }

        public TypeNumberTextEncodingAttribute(string encodingName)
        {
            Value = Encoding.GetEncoding(encodingName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextProviderAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.Provider;

        public object Value { get; }

        public TypeNumberTextProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextNumberStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.NumberStyle;

        public object Value { get; }

        public TypeNumberTextNumberStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextDecimalStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.DecimalStyle;

        public object Value { get; }

        public TypeNumberTextDecimalStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextPaddingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.Padding;

        public object Value { get; }

        public TypeNumberTextPaddingAttribute(Padding value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => NumberTextParameter.Filler;

        public object Value { get; }

        public TypeNumberTextFillerAttribute(byte value)
        {
            Value = value;
        }
    }
}
