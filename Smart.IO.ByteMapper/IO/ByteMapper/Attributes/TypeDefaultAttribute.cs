namespace Smart.IO.ByteMapper.Attributes
{
    using System;
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
    public sealed class TypeZeroFillAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.ZeroFill;

        public object Value { get; }

        public TypeZeroFillAttribute(bool value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TypeUseGroupingAttribute : Attribute, ITypeDefaultAttribute
    {
        public string Key => Parameter.UseGrouping;

        public object Value { get; }

        public TypeUseGroupingAttribute(bool value)
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
