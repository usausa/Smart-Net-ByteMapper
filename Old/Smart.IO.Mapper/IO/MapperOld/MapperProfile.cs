namespace Smart.IO.MapperOld
{
    /// <summary>
    ///
    /// </summary>
    public abstract class MapperProfile : IMapperProfile
    {
        /// <summary>
        ///
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///
        /// </summary>
        protected MapperProfile()
        {
            Name = string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        protected MapperProfile(string name)
        {
            Name = name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        public abstract void Configure(IMapperConfigurationExpresion config);
    }
}
