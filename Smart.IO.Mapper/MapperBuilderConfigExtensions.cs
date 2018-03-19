namespace Smart.IO.Mapper
{
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

        public static MapperBuilderConfig DefaultTrim(this MapperBuilderConfig config, bool value)
        {
            return config.AddParameter(DefaultParameter.Trim, value);
        }

        public static MapperBuilderConfig DefaultPadding(this MapperBuilderConfig config, Padding value)
        {
            return config.AddParameter(DefaultParameter.Padding, value);
        }

        public static MapperBuilderConfig DefaultFiller(this MapperBuilderConfig config, byte value)
        {
            return config.AddParameter(DefaultParameter.Filler, value);
        }

        public static MapperBuilderConfig DefaultEndian(this MapperBuilderConfig config, Endian value)
        {
            return config.AddParameter(DefaultParameter.Endian, value);
        }
    }
}
