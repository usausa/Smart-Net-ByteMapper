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
    public sealed class TypeNumberEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberEncoding;

        public object Value { get; }

        public TypeNumberEncodingAttribute(int codePage)
        {
            Value = Encoding.GetEncoding(codePage);
        }

        public TypeNumberEncodingAttribute(string encodingName)
        {
            Value = Encoding.GetEncoding(encodingName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeEncodingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeEncoding;

        public object Value { get; }

        public TypeDateTimeEncodingAttribute(int codePage)
        {
            Value = Encoding.GetEncoding(codePage);
        }

        public TypeDateTimeEncodingAttribute(string encodingName)
        {
            Value = Encoding.GetEncoding(encodingName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberProviderAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberProvider;

        public object Value { get; }

        public TypeNumberProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeProviderAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeProvider;

        public object Value { get; }

        public TypeDateTimeProviderAttribute(Culture value)
        {
            Value = value.ToCultureInfo();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeNumberStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberStyle;

        public object Value { get; }

        public TypeNumberStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDecimalStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DecimalStyle;

        public object Value { get; }

        public TypeDecimalStyleAttribute(NumberStyles value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeDateTimeStyleAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.DateTimeStyle;

        public object Value { get; }

        public TypeDateTimeStyleAttribute(DateTimeStyles value)
        {
            Value = value;
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
    public sealed class TypeNumberPaddingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberPadding;

        public object Value { get; }

        public TypeNumberPaddingAttribute(Padding value)
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
    public sealed class TypeNumberFillerAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.NumberFiller;

        public object Value { get; }

        public TypeNumberFillerAttribute(byte value)
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
}
