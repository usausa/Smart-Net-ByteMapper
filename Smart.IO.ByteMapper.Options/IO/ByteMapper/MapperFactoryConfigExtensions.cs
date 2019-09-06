namespace Smart.IO.ByteMapper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
    public static class OptionsMapperFactoryConfigExtensions
    {
        public static MapperFactoryConfig UseOptionsDefault(this MapperFactoryConfig config)
        {
            config.DefaultNumberPadding(Padding.Left);
            config.DefaultZeroFill(false);
            config.DefaultUseGrouping(false);
            config.DefaultNumberFiller(0x20);
            return config;
        }

        public static MapperFactoryConfig DefaultNumberPadding(this MapperFactoryConfig config, Padding value)
        {
            return config.AddParameter(OptionsParameter.NumberPadding, value);
        }

        public static MapperFactoryConfig DefaultZeroFill(this MapperFactoryConfig config, bool value)
        {
            return config.AddParameter(OptionsParameter.ZeroFill, value);
        }

        public static MapperFactoryConfig DefaultUseGrouping(this MapperFactoryConfig config, bool value)
        {
            return config.AddParameter(OptionsParameter.UseGrouping, value);
        }

        public static MapperFactoryConfig DefaultNumberFiller(this MapperFactoryConfig config, byte value)
        {
            return config.AddParameter(OptionsParameter.NumberFiller, value);
        }

        public static MapperFactoryConfig DefaultUnicodeFiller(this MapperFactoryConfig config, char value)
        {
            return config.AddParameter(OptionsParameter.UnicodeFiller, value);
        }
    }
}
