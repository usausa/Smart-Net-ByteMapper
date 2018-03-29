namespace Smart.IO.Mapper
{
    using System.Globalization;
    using System.Text;

    public static class ByteMapperConfigExtensions
    {
        public static ByteMapper ToByteMapper(this ByteMapperConfig config)
        {
            return new ByteMapper(config);
        }

        public static ByteMapperConfig DefaultDelimiter(this ByteMapperConfig config, byte[] value)
        {
            return config.AddParameter(Parameter.Delimiter, value);
        }

        public static ByteMapperConfig DefaultTextEncoding(this ByteMapperConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.TextEncoding, value);
        }

        public static ByteMapperConfig DefaultNumberEncoding(this ByteMapperConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.NumberEncoding, value);
        }

        public static ByteMapperConfig DefaultDateTimeEncoding(this ByteMapperConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.DateTimeEncoding, value);
        }

        public static ByteMapperConfig DefaultNumberProvider(this ByteMapperConfig config, NumberFormatInfo value)
        {
            return config.AddParameter(Parameter.NumberProvider, value);
        }

        public static ByteMapperConfig DefaultNumberStyle(this ByteMapperConfig config, NumberStyles value)
        {
            return config.AddParameter(Parameter.NumberStyle, value);
        }

        public static ByteMapperConfig DefaultDecimalStyle(this ByteMapperConfig config, NumberStyles value)
        {
            return config.AddParameter(Parameter.DecimalStyle, value);
        }

        public static ByteMapperConfig DefaultDateTimeProvider(this ByteMapperConfig config, DateTimeFormatInfo value)
        {
            return config.AddParameter(Parameter.DateTimeProvider, value);
        }

        public static ByteMapperConfig DefaultDateTimeStyle(this ByteMapperConfig config, DateTimeStyles value)
        {
            return config.AddParameter(Parameter.DateTimeStyle, value);
        }

        public static ByteMapperConfig DefaultTrim(this ByteMapperConfig config, bool value)
        {
            return config.AddParameter(Parameter.Trim, value);
        }

        public static ByteMapperConfig DefaultTextPadding(this ByteMapperConfig config, Padding value)
        {
            return config.AddParameter(Parameter.TextPadding, value);
        }

        public static ByteMapperConfig DefaultNumberPadding(this ByteMapperConfig config, Padding value)
        {
            return config.AddParameter(Parameter.NumberPadding, value);
        }

        public static ByteMapperConfig DefaultFiller(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(Parameter.Filler, value);
        }

        public static ByteMapperConfig DefaultTextFiller(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(Parameter.TextFiller, value);
        }

        public static ByteMapperConfig DefaultNumberFiller(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(Parameter.NumberFiller, value);
        }

        public static ByteMapperConfig DefaultEndian(this ByteMapperConfig config, Endian value)
        {
            return config.AddParameter(Parameter.Endian, value);
        }

        public static ByteMapperConfig DefaultTrueValue(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(Parameter.TrueValue, value);
        }

        public static ByteMapperConfig DefaultFalseValue(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(Parameter.FalseValue, value);
        }
    }
}
