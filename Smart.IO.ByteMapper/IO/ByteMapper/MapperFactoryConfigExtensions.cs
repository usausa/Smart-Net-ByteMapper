namespace Smart.IO.ByteMapper
{
    using System;
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

        public static MapperFactoryConfig DefaultZeroFill(this MapperFactoryConfig config, bool value)
        {
            return config.AddParameter(Parameter.ZeroFill, value);
        }

        public static MapperFactoryConfig DefaultUseGrouping(this MapperFactoryConfig config, bool value)
        {
            return config.AddParameter(Parameter.UseGrouping, value);
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

        public static MapperFactoryConfig DefaultDateTimeKind(this MapperFactoryConfig config, DateTimeKind value)
        {
            return config.AddParameter(Parameter.DateTimeKind, value);
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
