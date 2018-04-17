namespace Smart.IO.ByteMapper
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class OptionsMapperFactoryConfigExtensions
    {
        public static MapperFactoryConfig UseOptionsDefault(this MapperFactoryConfig config)
        {
            config.DefaultDateTimeTextEncoding(Encoding.ASCII);
            config.DefaultDateTimeTextProvider(CultureInfo.InvariantCulture);
            config.DefaultDateTimeTextStyle(DateTimeStyles.None);

            config.DefaultNumberTextEncoding(Encoding.ASCII);
            config.DefaultNumberTextProvider(CultureInfo.InvariantCulture);
            config.DefaultNumberTextNumberStyle(NumberStyles.Integer);
            config.DefaultNumberTextDecimalStyle(NumberStyles.Number);
            config.DefaultNumberTextPadding(Padding.Left);
            config.DefaultNumberTextFiller(0x20);

            return config;
        }

        public static MapperFactoryConfig DefaultDateTimeTextEncoding(this MapperFactoryConfig config, Encoding value)
        {
            return config.AddParameter(DateTimeTextParameter.Encoding, value);
        }

        public static MapperFactoryConfig DefaultDateTimeTextProvider(this MapperFactoryConfig config, IFormatProvider value)
        {
            return config.AddParameter(DateTimeTextParameter.Provider, value);
        }

        public static MapperFactoryConfig DefaultDateTimeTextStyle(this MapperFactoryConfig config, DateTimeStyles value)
        {
            return config.AddParameter(DateTimeTextParameter.Style, value);
        }

        public static MapperFactoryConfig DefaultNumberTextEncoding(this MapperFactoryConfig config, Encoding value)
        {
            return config.AddParameter(NumberTextParameter.Encoding, value);
        }

        public static MapperFactoryConfig DefaultNumberTextProvider(this MapperFactoryConfig config, IFormatProvider value)
        {
            return config.AddParameter(NumberTextParameter.Provider, value);
        }

        public static MapperFactoryConfig DefaultNumberTextNumberStyle(this MapperFactoryConfig config, NumberStyles value)
        {
            return config.AddParameter(NumberTextParameter.NumberStyle, value);
        }

        public static MapperFactoryConfig DefaultNumberTextDecimalStyle(this MapperFactoryConfig config, NumberStyles value)
        {
            return config.AddParameter(NumberTextParameter.DecimalStyle, value);
        }

        public static MapperFactoryConfig DefaultNumberTextPadding(this MapperFactoryConfig config, Padding value)
        {
            return config.AddParameter(NumberTextParameter.Padding, value);
        }

        public static MapperFactoryConfig DefaultNumberTextFiller(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(NumberTextParameter.Filler, value);
        }
    }
}
