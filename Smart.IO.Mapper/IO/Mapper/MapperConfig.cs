namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Smart.IO.Mapper.Mappers;

    /// <summary>
    ///
    /// </summary>
    internal class MapperConfig : IMapperConfig
    {
        private readonly Dictionary<Tuple<string, Type>, ITypeMapper> typeEntries = new Dictionary<Tuple<string, Type>, ITypeMapper>();

        /// <summary>
        ///
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        ///
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ITypeMapper FindTypeMapper(string profile, Type type)
        {
            ITypeMapper typeMapper;
            return typeEntries.TryGetValue(Tuple.Create(profile ?? string.Empty, type), out typeMapper) ? typeMapper : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        /// <param name="typeMapper"></param>
        public void AddTypeMapper(string profile, Type type, ITypeMapper typeMapper)
        {
            typeEntries[Tuple.Create(profile ?? string.Empty, type)] = typeMapper;
        }
    }
}
