namespace Smart.IO.ByteMapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDelimiterAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.Delimiter;

        public object Value { get; }

        public TypeDelimiterAttribute(params byte[] value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.Encoding;

        public object Value { get; }

        public TypeEncodingAttribute(int codePage)
        {
            Value = Encoding.GetEncoding(codePage);
        }

        public TypeEncodingAttribute(string encodingName)
        {
            Value = Encoding.GetEncoding(encodingName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeTrimAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.Trim;

        public object Value { get; }

        public TypeTrimAttribute(bool value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeTextPaddingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.TextPadding;

        public object Value { get; }

        public TypeTextPaddingAttribute(Padding value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.Filler;

        public object Value { get; }

        public TypeFillerAttribute(byte value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeTextFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.TextFiller;

        public object Value { get; }

        public TypeTextFillerAttribute(byte value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeEndianAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.Endian;

        public object Value { get; }

        public TypeEndianAttribute(Endian value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeKindAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeKind;

        public object Value { get; }

        public TypeDateTimeKindAttribute(DateTimeKind value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeTrueValueAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.TrueValue;

        public object Value { get; }

        public TypeTrueValueAttribute(byte value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeFalseValueAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.FalseValue;

        public object Value { get; }

        public TypeFalseValueAttribute(byte value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeTextEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeTextEncoding;

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
        public string Key => Parameter.DateTimeTextProvider;

        public object Value { get; }

        public TypeDateTimeTextProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeTextStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeTextStyle;

        public object Value { get; }

        public TypeDateTimeTextStyleAttribute(DateTimeStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberTextEncoding;

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
        public string Key => Parameter.NumberTextProvider;

        public object Value { get; }

        public TypeNumberTextProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextNumberStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberTextNumberStyle;

        public object Value { get; }

        public TypeNumberTextNumberStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextDecimalStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberTextDecimalStyle;

        public object Value { get; }

        public TypeNumberTextDecimalStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextPaddingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberTextPadding;

        public object Value { get; }

        public TypeNumberTextPaddingAttribute(Padding value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberTextFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberTextFiller;

        public object Value { get; }

        public TypeNumberTextFillerAttribute(byte value)
        {
            Value = value;
        }
    }
}
