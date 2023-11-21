namespace Smart.IO.ByteMapper;

using System.Globalization;
using System.Text;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
public static class MapperFactoryConfigExtensions
{
    public static MapperFactory ToMapperFactory(this MapperFactoryConfig config)
    {
        return new(config);
    }

    public static MapperFactoryConfig DefaultDelimiter(this MapperFactoryConfig config, params byte[] value)
    {
        return config.AddParameter(Parameter.Delimiter, value);
    }

    public static MapperFactoryConfig DefaultEncoding(this MapperFactoryConfig config, Encoding value)
    {
        return config.AddParameter(Parameter.Encoding, value);
    }

    public static MapperFactoryConfig DefaultTrim(this MapperFactoryConfig config, bool value)
    {
        return config.AddParameter(Parameter.Trim, value);
    }

    public static MapperFactoryConfig DefaultTextPadding(this MapperFactoryConfig config, Padding value)
    {
        return config.AddParameter(Parameter.TextPadding, value);
    }

    public static MapperFactoryConfig DefaultFiller(this MapperFactoryConfig config, byte value)
    {
        return config.AddParameter(Parameter.Filler, value);
    }

    public static MapperFactoryConfig DefaultTextFiller(this MapperFactoryConfig config, byte value)
    {
        return config.AddParameter(Parameter.TextFiller, value);
    }

    public static MapperFactoryConfig DefaultEndian(this MapperFactoryConfig config, Endian value)
    {
        return config.AddParameter(Parameter.Endian, value);
    }

    public static MapperFactoryConfig DefaultDateTimeKind(this MapperFactoryConfig config, DateTimeKind value)
    {
        return config.AddParameter(Parameter.DateTimeKind, value);
    }

    public static MapperFactoryConfig DefaultTrueValue(this MapperFactoryConfig config, byte value)
    {
        return config.AddParameter(Parameter.TrueValue, value);
    }

    public static MapperFactoryConfig DefaultFalseValue(this MapperFactoryConfig config, byte value)
    {
        return config.AddParameter(Parameter.FalseValue, value);
    }

    public static MapperFactoryConfig DefaultDateTimeTextEncoding(this MapperFactoryConfig config, Encoding value)
    {
        return config.AddParameter(Parameter.DateTimeTextEncoding, value);
    }

    public static MapperFactoryConfig DefaultDateTimeTextProvider(this MapperFactoryConfig config, IFormatProvider value)
    {
        return config.AddParameter(Parameter.DateTimeTextProvider, value);
    }

    public static MapperFactoryConfig DefaultDateTimeTextStyle(this MapperFactoryConfig config, DateTimeStyles value)
    {
        return config.AddParameter(Parameter.DateTimeTextStyle, value);
    }

    public static MapperFactoryConfig DefaultNumberTextEncoding(this MapperFactoryConfig config, Encoding value)
    {
        return config.AddParameter(Parameter.NumberTextEncoding, value);
    }

    public static MapperFactoryConfig DefaultNumberTextProvider(this MapperFactoryConfig config, IFormatProvider value)
    {
        return config.AddParameter(Parameter.NumberTextProvider, value);
    }

    public static MapperFactoryConfig DefaultNumberTextNumberStyle(this MapperFactoryConfig config, NumberStyles value)
    {
        return config.AddParameter(Parameter.NumberTextNumberStyle, value);
    }

    public static MapperFactoryConfig DefaultNumberTextDecimalStyle(this MapperFactoryConfig config, NumberStyles value)
    {
        return config.AddParameter(Parameter.NumberTextDecimalStyle, value);
    }

    public static MapperFactoryConfig DefaultNumberTextPadding(this MapperFactoryConfig config, Padding value)
    {
        return config.AddParameter(Parameter.NumberTextPadding, value);
    }

    public static MapperFactoryConfig DefaultNumberTextFiller(this MapperFactoryConfig config, byte value)
    {
        return config.AddParameter(Parameter.NumberTextFiller, value);
    }
}
