namespace Smart.IO.MapperOld
{
    using Smart.IO.MapperOld.Configuration;

    /// <summary>
    ///
    /// </summary>
    public class MapperConfigBuilder
    {
        private readonly IMapperProfile[] profiles;

        /// <summary>
        ///
        /// </summary>
        /// <param name="profiles"></param>
        public MapperConfigBuilder(params IMapperProfile[] profiles)
        {
            this.profiles = profiles;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IMapperConfig Build()
        {
            var mapperConfig = new MapperConfig();

            foreach (var profile in profiles)
            {
                profile.Configure(new MapperConfigurationExpression(profile.Name, mapperConfig));
            }

            return mapperConfig;
        }
    }
}
