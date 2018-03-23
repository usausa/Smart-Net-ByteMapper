namespace Smart.IO.Mapper
{
    using System.Globalization;
    using System.Text;

    public static class MapperBuilderConfigExtensions
    {
        public static MapperBuilderConfig DefaultDelimiter(this MapperBuilderConfig config, byte[] value)
        {
            return config.AddParameter(DefaultParameter.Delimiter, value);
        }

        public static MapperBuilderConfig DefaultEncoding(this MapperBuilderConfig config, Encoding value)
        {
            return config.AddParameter(DefaultParameter.Encoding, value);
        }

        public static MapperBuilderConfig DefaultNumberFormat(this MapperBuilderConfig config, NumberFormatInfo value)
        {
            return config.AddParameter(DefaultParameter.NumberFormat, value);
        }

        public static MapperBuilderConfig DefaultNumberStyle(this MapperBuilderConfig config, NumberStyles value)
        {
            return config.AddParameter(DefaultParameter.NumberStyle, value);
        }

        public static MapperBuilderConfig DefaultDecimalStyle(this MapperBuilderConfig config, NumberStyles value)
        {
            return config.AddParameter(DefaultParameter.DecimalStyle, value);
        }

        public static MapperBuilderConfig DefaultDateTimeFormat(this MapperBuilderConfig config, DateTimeFormatInfo value)
        {
            return config.AddParameter(DefaultParameter.DateTimeFormat, value);
        }

        public static MapperBuilderConfig DefaultDateTimeStyle(this MapperBuilderConfig config, DateTimeStyles value)
        {
            return config.AddParameter(DefaultParameter.DateTimeStyle, value);
        }

        public static MapperBuilderConfig DefaultTrim(this MapperBuilderConfig config, bool value)
        {
            return config.AddParameter(DefaultParameter.Trim, value);
        }

        public static MapperBuilderConfig DefaultTextPadding(this MapperBuilderConfig config, Padding value)
        {
            return config.AddParameter(DefaultParameter.TextPadding, value);
        }

        public static MapperBuilderConfig DefaultNumberPadding(this MapperBuilderConfig config, Padding value)
        {
            return config.AddParameter(DefaultParameter.NumberPadding, value);
        }

        public static MapperBuilderConfig DefaultTextFiller(this MapperBuilderConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.TextFiller, value);
        }

        public static MapperBuilderConfig DefaultNumberFiller(this MapperBuilderConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.NumberFiller, value);
        }

        public static MapperBuilderConfig DefaultEndian(this MapperBuilderConfig config, Endian value)
        {
            return config.AddParameter(DefaultParameter.Endian, value);
        }

        public static MapperBuilderConfig DefaultTrue(this MapperBuilderConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.True, value);
        }

        public static MapperBuilderConfig DefaultFalse(this MapperBuilderConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.False, value);
        }
    }
}
