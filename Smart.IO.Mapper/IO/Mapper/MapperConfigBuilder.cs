namespace Smart.IO.Mapper
{
    using Smart.IO.Mapper.Configuration;

    /// <summary>
    ///
    /// </summary>
    public class MapperConfigBuilder
    {
        private readonly IMapperProfile profile;

        /// <summary>
        ///
        /// </summary>
        /// <param name="profile"></param>
        public MapperConfigBuilder(IMapperProfile profile)
        {
            this.profile = profile;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IMapperConfig Build()
        {
            var mapperConfig = new MapperConfig();

            profile.Configure(new MapperConfigurationExpression(mapperConfig));

            return mapperConfig;
        }
    }
}
