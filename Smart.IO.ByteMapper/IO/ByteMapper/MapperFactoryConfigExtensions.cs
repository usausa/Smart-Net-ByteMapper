namespace Smart.IO.ByteMapper
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class MapperFactoryConfigExtensions
    {
        public static MapperFactory ToMapperFactory(this MapperFactoryConfig config)
        {
            return new MapperFactory(config);
        }

        public static MapperFactoryConfig DefaultDelimiter(this MapperFactoryConfig config, params byte[] value)
        {
            return config.AddParameter(Parameter.Delimiter, value);
        }

        public static MapperFactoryConfig DefaultEncoding(this MapperFactoryConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.Encoding, value);
        }

        public static MapperFactoryConfig DefaultNumberEncoding(this MapperFactoryConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.NumberEncoding, value);
        }

        public static MapperFactoryConfig DefaultDateTimeEncoding(this MapperFactoryConfig config, Encoding value)
        {
            return config.AddParameter(Parameter.DateTimeEncoding, value);
        }

        public static MapperFactoryConfig DefaultNumberProvider(this MapperFactoryConfig config, IFormatProvider value)
        {
            return config.AddParameter(Parameter.NumberProvider, value);
        }

        public static MapperFactoryConfig DefaultDateTimeProvider(this MapperFactoryConfig config, IFormatProvider value)
        {
            return config.AddParameter(Parameter.DateTimeProvider, value);
        }

        public static MapperFactoryConfig DefaultNumberStyle(this MapperFactoryConfig config, NumberStyles value)
        {
            return config.AddParameter(Parameter.NumberStyle, value);
        }

        public static MapperFactoryConfig DefaultDecimalStyle(this MapperFactoryConfig config, NumberStyles value)
        {
            return config.AddParameter(Parameter.DecimalStyle, value);
        }

        public static MapperFactoryConfig DefaultDateTimeStyle(this MapperFactoryConfig config, DateTimeStyles value)
        {
            return config.AddParameter(Parameter.DateTimeStyle, value);
        }

        public static MapperFactoryConfig DefaultTrim(this MapperFactoryConfig config, bool value)
        {
            return config.AddParameter(Parameter.Trim, value);
        }

        public static MapperFactoryConfig DefaultTextPadding(this MapperFactoryConfig config, Padding value)
        {
            return config.AddParameter(Parameter.TextPadding, value);
        }

        public static MapperFactoryConfig DefaultNumberPadding(this MapperFactoryConfig config, Padding value)
        {
            return config.AddParameter(Parameter.NumberPadding, value);
        }

        public static MapperFactoryConfig DefaultFiller(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(Parameter.Filler, value);
        }

        public static MapperFactoryConfig DefaultTextFiller(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(Parameter.TextFiller, value);
        }

        public static MapperFactoryConfig DefaultNumberFiller(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(Parameter.NumberFiller, value);
        }

        public static MapperFactoryConfig DefaultEndian(this MapperFactoryConfig config, Endian value)
        {
            return config.AddParameter(Parameter.Endian, value);
        }

        public static MapperFactoryConfig DefaultTrueValue(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(Parameter.TrueValue, value);
        }

        public static MapperFactoryConfig DefaultFalseValue(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(Parameter.FalseValue, value);
        }
    }
}
