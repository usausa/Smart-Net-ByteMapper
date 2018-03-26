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
            return config.AddParameter(DefaultParameter.Delimiter, value);
        }

        public static ByteMapperConfig DefaultTextEncoding(this ByteMapperConfig config, Encoding value)
        {
            return config.AddParameter(DefaultParameter.TextEncoding, value);
        }

        public static ByteMapperConfig DefaultNumberEncoding(this ByteMapperConfig config, Encoding value)
        {
            return config.AddParameter(DefaultParameter.NumberEncoding, value);
        }

        public static ByteMapperConfig DefaultNumberFormat(this ByteMapperConfig config, NumberFormatInfo value)
        {
            return config.AddParameter(DefaultParameter.NumberFormat, value);
        }

        public static ByteMapperConfig DefaultNumberStyle(this ByteMapperConfig config, NumberStyles value)
        {
            return config.AddParameter(DefaultParameter.NumberStyle, value);
        }

        public static ByteMapperConfig DefaultDecimalStyle(this ByteMapperConfig config, NumberStyles value)
        {
            return config.AddParameter(DefaultParameter.DecimalStyle, value);
        }

        public static ByteMapperConfig DefaultDateTimeFormat(this ByteMapperConfig config, DateTimeFormatInfo value)
        {
            return config.AddParameter(DefaultParameter.DateTimeFormat, value);
        }

        public static ByteMapperConfig DefaultDateTimeStyle(this ByteMapperConfig config, DateTimeStyles value)
        {
            return config.AddParameter(DefaultParameter.DateTimeStyle, value);
        }

        public static ByteMapperConfig DefaultTrim(this ByteMapperConfig config, bool value)
        {
            return config.AddParameter(DefaultParameter.Trim, value);
        }

        public static ByteMapperConfig DefaultTextPadding(this ByteMapperConfig config, Padding value)
        {
            return config.AddParameter(DefaultParameter.TextPadding, value);
        }

        public static ByteMapperConfig DefaultNumberPadding(this ByteMapperConfig config, Padding value)
        {
            return config.AddParameter(DefaultParameter.NumberPadding, value);
        }

        public static ByteMapperConfig DefaultTextFiller(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.TextFiller, value);
        }

        public static ByteMapperConfig DefaultNumberFiller(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.NumberFiller, value);
        }

        public static ByteMapperConfig DefaultEndian(this ByteMapperConfig config, Endian value)
        {
            return config.AddParameter(DefaultParameter.Endian, value);
        }

        public static ByteMapperConfig DefaultTrueValue(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.TrueValue, value);
        }

        public static ByteMapperConfig DefaultFalseValue(this ByteMapperConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.FalseValue, value);
        }
    }
}
